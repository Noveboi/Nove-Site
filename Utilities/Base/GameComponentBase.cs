using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using LearningBlazor.Hubs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using LearningBlazor.Utilities.TicTacToe;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace LearningBlazor.Utilities.Base;
/// <summary>
/// Base class for Razor Components that are applets for games. 
/// <para>
///		This base class is concerned with providing such Components with basic methods for communicating 
///		with <see cref="GameHubBase{TGame, TPlayer}"/> instances as well as providing some basic variables such as <see cref="GameState"/>
/// </para>
/// </summary>
public class GameComponentBase<TPlayer> : ComponentBase, IAsyncDisposable where TPlayer : PlayerModel
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;
	[Inject]
	private ProtectedSessionStorage SessionStorage { get; set; } = default!;
	[Inject]
	private IJSRuntime JS { get; set; } = default!;

	[Inject]
	protected ILogger<GameComponentBase<TPlayer>> Logger { get; set; } = default!;
	protected GameHubProtocol Protocol => GameHubProtocol.Singleton;

	private HubConnection? hubConnection;

	protected GameState gameState = GameState.InLobby;
	protected List<GameModel> lobbyGames = [];
	protected List<PlayerModel> gamePlayers = [];
	protected TPlayer? selfPlayer;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
		if (firstRender)
		{
			await JS.InvokeVoidAsync("registerListeners", DotNetObjectReference.Create(this));
		}
    }

    protected async Task HandleExit()
	{
		Logger.LogInformation("Exiting by means of redirect");
		if (hubConnection is not null)
			await hubConnection.StopAsync();

		NavManager.NavigateTo("/");
	}

	protected async Task BuildHubConnection(string hubRelativeUri)
	{
		hubConnection = new HubConnectionBuilder()
				.WithUrl(NavManager.ToAbsoluteUri(hubRelativeUri))
				.WithAutomaticReconnect()
				.Build();

		// Set up receivers
		hubConnection.On(Protocol[Receivers.OnBeginGame], OnBeginGame);
		hubConnection.On<string>(Protocol[Receivers.GetGameList], GetGameList);
		hubConnection.On<string>(Protocol[Receivers.OtherConnected], OtherConnected);
		hubConnection.On<string>(Protocol[Receivers.SelfConnected], SelfConnected);
		hubConnection.On<string>(Protocol[Receivers.OtherDisconnected], OtherDisconnected);
		hubConnection.On<string>(Protocol[Receivers.GetPlayerModel], GetPlayerModel);
		hubConnection.On<string>(Protocol[Receivers.UpdateGameList], UpdateGameList);

		await hubConnection.StartAsync();

		// Get username from Session Storage
		var result = await SessionStorage.GetAsync<string>("username");
		string username = result.Success
			? result.Value ?? throw new Exception("Retrieved NULL username from Session Storage!")
			: "SES_STORAGE_FAILURE";

#if DEBUG
		Thread.CurrentThread.Name = $"{username}'s thread";
#endif

		await hubConnection.SendAsync(Protocol[Senders.CreatePlayer], username);

		Logger.LogInformation("Succesfully built hub connection for player: \"{username}\"", username);
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
	private async Task OnBeginGame()
	{
		gameState = GameState.Playing;
		await InvokeAsync(StateHasChanged);
	}

	/// <summary>
	/// This method will fire when ANOTHER player has connected to the <see cref="GameModel"/> that the client is 
	/// currently in
	/// </summary>
	public virtual async Task OtherConnected(string json)
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(Protocol[Receivers.OtherConnected]);

		var player = JsonConvert.DeserializeObject<PlayerModel>(json)
			?? throw new Exception("Deserialized into NULL obhect when trying to get Player!");

		gamePlayers.Add(player);
	} 

	protected virtual async Task SelfConnected(string json)
	{
		var playerList = JsonConvert.DeserializeObject<List<PlayerModel>>(json)
			?? throw new Exception("Deserialized into NULL object when trying to get Player List!");

		gamePlayers = playerList;
		gamePlayers.Add(selfPlayer!);
		await InvokeAsync(StateHasChanged);
	}

	protected virtual async Task OtherDisconnected(string playerId)
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(Protocol[Senders.OtherPlayerDisconnected], playerId);

		var playerToRemove = gamePlayers.FirstOrDefault(p => p.Id == playerId)
			?? throw new Exception("Couldn't find player to remove from gamePlayers");

		gamePlayers.Remove(playerToRemove);
	}

	private async Task GetGameList(string json)
	{
		var gameList = JsonConvert.DeserializeObject<List<GameModel>>(json)
			?? throw new Exception("Deserialized into NULL object when trying to get Game List!");

		lobbyGames = gameList;
		await InvokeAsync(StateHasChanged);
	}

	private Task GetPlayerModel(string json)
	{
		var player = JsonConvert.DeserializeObject<TPlayer>(json)
			?? throw new Exception("Deserialized into NULL object when trying to get Player!");

		selfPlayer = player;
		return Task.CompletedTask;
	}

	private async Task UpdateGameList(string json)
	{
		var game = JsonConvert.DeserializeObject<GameModel>(json)
			?? throw new Exception("Deserialized into NULL object when trying to get Game!");

		lobbyGames.Add(game);
		await InvokeAsync(StateHasChanged);
	}

	protected async Task CreateNewGame()
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(Protocol[Senders.CreateNewGame]);

		gameState = GameState.Waiting;
		gamePlayers.Add(selfPlayer!);

		await InvokeAsync(StateHasChanged);
	}

	protected async Task OnGameJoinClick(GameModel game)
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(Protocol[Senders.PlayerJoinGame], game.NameId);
	}

	[JSInvokable]
	public async Task OnBrowserTabClose()
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(Protocol[Senders.OnBrowserClose]);
	}

	public async ValueTask DisposeAsync()
	{
		if (hubConnection is not null)
		{
			await JS.InvokeVoidAsync("unregisterListeners");
			await hubConnection.DisposeAsync();
			GC.SuppressFinalize(this);
		}
	}
}
