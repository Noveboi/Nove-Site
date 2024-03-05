using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using LearningBlazor.Hubs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using LearningBlazor.Utilities.TicTacToe;

namespace LearningBlazor.Utilities.Base;
/// <summary>
/// Base class for Razor Components that are applets for games. 
/// <para>
///		<see cref="GameComponent"/> is concerned with providing such Components with basic methods for communicating 
///		with <see cref="GameHub{TGame}"/> instances as well as providing some basic variables such as <see cref="GameState"/>
/// </para>
/// </summary>
public class GameComponent : ComponentBase, IAsyncDisposable
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;
	[Inject]
	private ProtectedSessionStorage SessionStorage { get; set; } = default!;
	private HubConnection? hubConnection;

	protected GameState gameState = GameState.InLobby;
	protected List<GameModel> lobbyGames = [];
	protected List<PlayerModel> gamePlayers = [];
	protected string username = string.Empty;

	public const string RECEIVERS_BEGIN_GAME = nameof(BeginGame);
	public const string RECEIVERS_CLIENT_CONNECTED = nameof(ClientConnected);
	public const string RECEIVERS_PLAYER_CONNECTED = nameof(PlayerConnected);
	public const string RECEIVERS_PLAYER_DISCONNECTED = nameof(PlayerDisconnected);
	public const string RECEIVERS_GET_GAME_LIST = nameof(GetGameList);
	public const string RECEIVERS_UPDATE_GAME_LIST = nameof(UpdateGameList);

	public const string SENDERS_JOIN_GAME = nameof(OnGameJoinClick);

	protected async Task HandleExit()
	{
		if (hubConnection is not null)
			await hubConnection.StopAsync();

		NavManager.NavigateTo("/");
	}

	protected async Task BuildHubConnection(string hubRelativeUri)
	{
		var result = await SessionStorage.GetAsync<string>("username");
		username = result.Success ? result.Value ?? throw new Exception("nuh uh") : "Result failed";

		hubConnection = new HubConnectionBuilder()
				.WithUrl(NavManager.ToAbsoluteUri(hubRelativeUri))
				.WithAutomaticReconnect()
				.Build();

		SetUpReceivers();
		await hubConnection.StartAsync();
		await hubConnection.SendAsync(TicTacToeHub.SENDER_CREATE_PLAYER, username);
	}

	private void SetUpReceivers()
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(RECEIVERS_BEGIN_GAME, BeginGame);
		hubConnection.On<string>(RECEIVERS_GET_GAME_LIST, GetGameList);
		hubConnection.On<string>(RECEIVERS_PLAYER_CONNECTED, PlayerConnected);
		hubConnection.On<string>(RECEIVERS_CLIENT_CONNECTED, ClientConnected);
	}

	#region Receiver & Sender Wrappers
	protected void AddReceiver(string methodName, Func<Task> handler)
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(methodName, handler);
	}

	protected void AddReceiver<T1>(string methodName, Func<T1, Task> handler)
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(methodName, handler);
	}

	protected void AddReceiver<T1, T2>(string methodName, Func<T1, T2, Task> handler)
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(methodName, handler);
	}

	protected void AddReceiver<T1, T2, T3>(string methodName, Func<T1, T2, T3, Task> handler)
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(methodName, handler);
	}

	protected async Task SendToHub([CallerMemberName] string methodName = "")
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		await hubConnection.SendAsync(methodName);
	}

	protected async Task SendToHub(object? arg1, [CallerMemberName] string methodName = "")
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		await hubConnection.SendAsync(methodName, arg1);
	}

	protected async Task SendToHub(object? arg1, object? arg2, [CallerMemberName] string methodName = "")
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		await hubConnection.SendAsync(methodName, arg1, arg2);
	}

	protected async Task SendToHub(object? arg1, object? arg2, object? arg3, [CallerMemberName] string methodName = "")
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		await hubConnection.SendAsync(methodName, arg1, arg2, arg3);
	}
	#endregion

	/// <summary>
	/// Sets the <see cref="GameState"/> to <see cref="GameState.Playing"/> and forces UI re-render
	/// </summary>
	private async Task BeginGame()
	{
		gameState = GameState.Playing;
		await InvokeAsync(StateHasChanged);
	}

	/// <summary>
	/// This method will fire when another player has connected to the <see cref="GameModel"/> that the client is 
	/// currently in
	/// </summary>
	/// <param name="connectionId">The <see cref="HubCallerContext.ConnectionId"/></param>
	/// <param name="username">The player's username</param>
	/// <exception cref="NotImplementedException"></exception>
	protected virtual async Task PlayerConnected(string json)
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(RECEIVERS_PLAYER_CONNECTED);

		var player = JsonConvert.DeserializeObject<PlayerModel>(json)
			?? throw new Exception("Deserialized into NULL obhect when trying to get Player!");

		gamePlayers.Add(player);
	} 

	protected virtual async Task ClientConnected(string json)
	{
		var playerList = JsonConvert.DeserializeObject<List<PlayerModel>>(json)
			?? throw new Exception("Deserialized into NULL object when trying to get Player List!");

		gamePlayers = playerList;
		gamePlayers.Add(new PlayerModel(hubConnection!.ConnectionId!, username));
		await InvokeAsync(StateHasChanged);
	}

	protected virtual async Task PlayerDisconnected()
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(RECEIVERS_PLAYER_DISCONNECTED);

		gamePlayers.Clear();
	}

	private async void GetGameList(string json)
	{
		var gameList = JsonConvert.DeserializeObject<List<GameModel>>(json)
			?? throw new Exception("Deserialized into NULL object when trying to get Game List!");

		lobbyGames = gameList;
		await InvokeAsync(StateHasChanged);
	}

	private async void UpdateGameList(string json)
	{
		var game = JsonConvert.DeserializeObject<GameModel>(json)
			?? throw new Exception("Deserialized into NULL object when trying to get Game!");

		lobbyGames.Add(game);
		await InvokeAsync(StateHasChanged);
	}

	protected async Task CreateNewGame()
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(TicTacToeHub.SENDER_CREATE_NEW_GAME);

		gameState = GameState.Waiting;
		gamePlayers.Add(new PlayerModel(hubConnection!.ConnectionId!, username));

		await InvokeAsync(StateHasChanged);
	}

	protected async Task OnGameJoinClick(GameModel game)
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(TicTacToeHub.SENDER_PLAYER_JOIN, game.NameId);

	}

	public async ValueTask DisposeAsync()
	{
		if (hubConnection is not null)
		{
			await hubConnection.DisposeAsync();
			GC.SuppressFinalize(this);
		}
	}
}
