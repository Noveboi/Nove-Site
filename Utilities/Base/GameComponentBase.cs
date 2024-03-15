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
using Serilog;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;

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
	public GameComponentBase()
	{
		gamePlayers = new ReadOnlyCollection<PlayerModel>(playerList);
	}

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;
	[Inject]
	private IJSRuntime JS { get; set; } = default!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = default!;

	private HubConnection? hubConnection;
	private List<PlayerModel> playerList = [];

	// Backing field for GamePlayers property
	private readonly ReadOnlyCollection<PlayerModel> gamePlayers;

	protected GameHubProtocol Protocol => GameHubProtocol.Singleton;

	/// <summary>
	/// The GameState helps deriving components dynamically render the proper UI elements based on the state of the game.
	/// </summary>
	protected GameStates GameState = GameStates.Waiting;
	/// <summary>
	/// Maintains the other players in the Game the user is in. This list is automatically updated in the <see cref="GameComponentBase{TPlayer}"/> class.
	/// </summary>
	protected ReadOnlyCollection<PlayerModel> GamePlayers => gamePlayers;
	/// <summary>
	/// Provides information about the user such as the Username and the Connection ID
	/// </summary>
	protected TPlayer? Self;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
		if (firstRender)
		{
			await JS.InvokeVoidAsync("registerListeners", DotNetObjectReference.Create(this));
		}
    }

	/// <summary>
	/// Navigates the user back to the home page.
	/// </summary>
	/// <returns></returns>
    protected async Task HandleExit()
	{
		Log.Information("Exiting by means of controlled redirect");
		if (hubConnection is not null)
			await hubConnection.StopAsync();

		NavManager.NavigateTo("/");
	}

	/// <summary>
	/// Builds the <see cref="HubConnection"/> and sets up receivers (listeners) using <see cref="HubConnection.On(string, Type[], Func{object?[], object, Task}, object)"/>
	/// </summary>
	/// <param name="hubUriName">The registered name of the Hub in the application's services (see <see cref="Program"/>)</param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
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
	/// <summary>
	/// Wrapper for <see cref="HubConnection"/>.On().
	/// 
	/// <para>
	///		Receives a message from the <see cref="Hub"/> that makes the component run the method with the name 
	///		specified in <paramref name="methodName"/>. The <paramref name="handler"/> takes in 0 arguments.
	/// </para>
	/// </summary>
	/// <param name="methodName"> The name of the method declared in the Component </param>
	/// <param name="handler"> What actions to take when called. </param>
	/// <exception cref="Exception"></exception>
	protected void AddReceiver(string methodName, Func<Task> handler)
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(methodName, handler);
	}

	/// <summary>
	/// Wrapper for <see cref="HubConnection"/>.On().
	/// 
	/// <para>
	///		Receives a message from the <see cref="Hub"/> that makes the component run the method with the name 
	///		specified in <paramref name="methodName"/>. The <paramref name="handler"/> takes in 1 argument.
	/// </para>
	/// </summary>
	/// <param name="methodName"> The name of the method declared in the Component </param>
	/// <param name="handler"> What actions to take when called. </param>
	/// <exception cref="Exception"></exception>
	protected void AddReceiver<T1>(string methodName, Func<T1, Task> handler)
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(methodName, handler);
	}

	/// <summary>
	/// Wrapper for <see cref="HubConnection"/>.On().
	/// 
	/// <para>
	///		Receives a message from the <see cref="Hub"/> that makes the component run the method with the name 
	///		specified in <paramref name="methodName"/>. The <paramref name="handler"/> takes in 2 arguments.
	/// </para>
	/// </summary>
	/// <param name="methodName"> The name of the method declared in the Component </param>
	/// <param name="handler"> What actions to take when called. </param>
	/// <exception cref="Exception"></exception>
	protected void AddReceiver<T1, T2>(string methodName, Func<T1, T2, Task> handler)
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(methodName, handler);
	}

	/// <summary>
	/// Wrapper for <see cref="HubConnection"/>.On().
	/// 
	/// <para>
	///		Receives a message from the <see cref="Hub"/> that makes the component run the method with the name 
	///		specified in <paramref name="methodName"/>. The <paramref name="handler"/> takes in 3 arguments.
	/// </para>
	/// </summary>
	/// <param name="methodName"> The name of the method declared in the Component </param>
	/// <param name="handler"> What actions to take when called. </param>
	/// <exception cref="Exception"></exception>
	protected void AddReceiver<T1, T2, T3>(string methodName, Func<T1, T2, T3, Task> handler)
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		hubConnection.On(methodName, handler);
	}

	/// <summary>
	/// Wrapper for <see cref="HubConnection"/>.SendAsync().
	/// 
	/// <para>
	///		Sends a message to the <see cref="Hub"/>. <paramref name="methodName"/> tells the <see cref="Hub"/> what method to invoke.
	///		The method specified in <paramref name="methodName"/> must take in 0 arguments.
	/// </para>
	/// </summary>
	/// <param name="methodName"> The name of the method declared in the connected <see cref="Hub"/> </param>
	/// <exception cref="Exception"></exception>
	protected async Task SendToHub([CallerMemberName] string methodName = "")
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		await hubConnection.SendAsync(methodName);
	}

	/// <summary>
	/// Wrapper for <see cref="HubConnection"/>.SendAsync().
	/// 
	/// <para>
	///		Sends a message to the <see cref="Hub"/>. <paramref name="methodName"/> tells the <see cref="Hub"/> what method to invoke.
	///		The method specified in <paramref name="methodName"/> must take in 1 arguments.
	/// </para>
	/// </summary>
	/// <param name="methodName"> The name of the method declared in the connected <see cref="Hub"/> </param>
	/// <exception cref="Exception"></exception>
	protected async Task SendToHub(object? arg1, [CallerMemberName] string methodName = "")
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		await hubConnection.SendAsync(methodName, arg1);
	}

	/// <summary>
	/// Wrapper for <see cref="HubConnection"/>.SendAsync().
	/// 
	/// <para>
	///		Sends a message to the <see cref="Hub"/>. <paramref name="methodName"/> tells the <see cref="Hub"/> what method to invoke.
	///		The method specified in <paramref name="methodName"/> must take in 2 arguments.
	/// </para>
	/// </summary>
	/// <param name="methodName"> The name of the method declared in the connected <see cref="Hub"/> </param>
	/// <exception cref="Exception"></exception>
	protected async Task SendToHub(object? arg1, object? arg2, [CallerMemberName] string methodName = "")
	{
		if (hubConnection is null)
			throw new Exception("Hub connection was null when trying to wire up receivers for HubConnection.On()");

		await hubConnection.SendAsync(methodName, arg1, arg2);
	}

	/// <summary>
	/// Wrapper for <see cref="HubConnection"/>.SendAsync().
	/// 
	/// <para>
	///		Sends a message to the <see cref="Hub"/>. <paramref name="methodName"/> tells the <see cref="Hub"/> what method to invoke.
	///		The method specified in <paramref name="methodName"/> must take in 3 arguments.
	/// </para>
	/// </summary>
	/// <param name="methodName"> The name of the method declared in the connected <see cref="Hub"/> </param>
	/// <exception cref="Exception"></exception>
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
	/// currently in.
	/// <para>
	/// IMPORTANT NOTE: This method does NOT send the player object to the <see cref="Hub"/>!
	///		This is because the Hub can infer who the new player is thanks to the <see langword="static"/> "Games" instance
	/// </para>
	/// </summary>
	public virtual async Task OtherConnected(string json)
	{
		var player = JsonConvert.DeserializeObject<PlayerModel>(json)
			?? throw new Exception("Deserialized into NULL obhect when trying to get Player!");

		playerList.Add(player);

		if (hubConnection is not null)
			await hubConnection.InvokeAsync(Protocol[Senders.OtherPlayerConnected]);
	}

	/// <summary>
	/// Receive the <typeparamref name="TPlayer"/> object and the <see cref="List{T}"/> of <see cref="PlayerModel"/> in the game.
	/// <para>
	///		You can override this method to add any extra business logic before or after this base implementation.
	/// </para>
	/// </summary>
	/// <param name="playerJson">The <typeparamref name="TPlayer"/> object encoded in JSON</param>
	/// <param name="gamePlayersJson">The <see cref="List{T}"/> of <see cref="PlayerModel"/> objects encoded in JSON</param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	protected virtual async Task SelfConnected(string playerJson, string gamePlayersJson)
	{
		var player = JsonConvert.DeserializeObject<TPlayer>(playerJson)
			?? throw new Exception("Deserialized ino NULL object when trying to get player object!");

		// The JSON string here can be empty if the Game the user has joined has 0 players
		if (gamePlayersJson != string.Empty)
			playerList = JsonConvert.DeserializeObject<List<PlayerModel>>(gamePlayersJson)
				?? throw new Exception("Deserialized into NULL object when trying to get player list!");

		Self = player;
		playerList.Add(Self);

		Log.Information("Player {Name} executed method {Method}", Self.Name, nameof(SelfConnected));

		await InvokeAsync(StateHasChanged);
	}

	/// <summary>
	/// Receives the connection ID of the player who has disconnected and handles the removal of any reference of that player throughout the client.
	/// 
	/// <para>
	///		You can override this method and add any extra business logic before/after the base implementation.
	/// </para>
	/// </summary>
	/// <param name="playerId">The Connection ID of the player who has disconnected</param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	protected virtual async Task OtherDisconnected(string playerId)
	{
		// Notify the Hub that a player has disconnected
		if (hubConnection is not null)
			await hubConnection.SendAsync(Protocol[Senders.OtherPlayerDisconnected], playerId);

		var playerToRemove = playerList.FirstOrDefault(p => p.Id == playerId)
			?? throw new Exception("Couldn't find player to remove from gamePlayers");

		playerList.Remove(playerToRemove);
	}

	/// <summary>
	/// Invoked from JavaScript when the "beforeunload" event is fired in the browser.
	/// </summary>
	/// <returns></returns>
	[JSInvokable]
	public async Task OnBrowserTabClose()
	{
		if (hubConnection is not null)
			await hubConnection.SendAsync(Protocol[Senders.OnBrowserClose]);

		string playerName = Self != null ? Self.Name : "Unregistered Player";
		Log.Information("{Player} exited by closing tab/browser", playerName);
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
