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
///		with <see cref="GameHubBase{TGame, TPlayer}"/> instances as well as providing some basic variables such as <see cref="Base.GameStates"/>
/// </para>
/// </summary>
public class GameComponentBase<TPlayer> : ComponentBase, IAsyncDisposable where TPlayer : PlayerModel
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;
	[Inject]
	private IJSRuntime JS { get; set; } = default!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = default!;

	private HubConnection? hubConnection;

	[Inject]
	protected ILogger<GameComponentBase<TPlayer>> Logger { get; set; } = default!;
	protected GameHubProtocol Protocol => GameHubProtocol.Singleton;

	protected GameStates GameState = GameStates.Waiting;
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

	protected async Task SetUpHubConnection(string hubUriName)
	{
		hubConnection = ServiceProvider.GetKeyedService<HubConnection>(hubUriName)
			?? throw new Exception("Couldn't find hubConnection instance");

		// Set up receivers
		hubConnection.On(Protocol[Receivers.OnBeginGame], OnBeginGame);
		hubConnection.On<string>(Protocol[Receivers.OtherConnected], OtherConnected);
		hubConnection.On<string>(Protocol[Receivers.OtherDisconnected], OtherDisconnected);
		hubConnection.On<string, string>(Protocol[Receivers.SelfConnected], SelfConnected);

		// Let the hub know!
		await hubConnection.SendAsync(Protocol[Senders.ReadyToConnect]);
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
	/// Sets the <see cref="Base.GameStates"/> to <see cref="GameStates.Playing"/> and forces UI re-render
	/// </summary>
	private async Task OnBeginGame()
	{
		GameState = GameStates.Playing;
		await InvokeAsync(StateHasChanged);
	}

	/// <summary>
	/// This method will fire when ANOTHER player has connected to the <see cref="GameModel"/> that the client is 
	/// currently in
	/// </summary>
	public virtual async Task OtherConnected(string json)
	{
		var player = JsonConvert.DeserializeObject<PlayerModel>(json)
			?? throw new Exception("Deserialized into NULL obhect when trying to get Player!");

		gamePlayers.Add(player);

		if (hubConnection is not null)
			await hubConnection.InvokeAsync(Protocol[Senders.OtherPlayerConnected]);
	} 

	protected virtual async Task SelfConnected(string playerJson, string gamePlayersJson)
	{
		var player = JsonConvert.DeserializeObject<TPlayer>(playerJson)
			?? throw new Exception("Deserialized ino NULL object when trying to get player object!");
		if (gamePlayersJson != string.Empty)
			gamePlayers = JsonConvert.DeserializeObject<List<PlayerModel>>(gamePlayersJson)
				?? throw new Exception("Deserialized into NULL object when trying to get player list!");

		selfPlayer = player;
		gamePlayers.Add(selfPlayer);

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
			GC.SuppressFinalize(this);
		}
	}
}
