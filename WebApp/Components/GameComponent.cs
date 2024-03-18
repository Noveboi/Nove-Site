using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using WebApp.Hubs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Games.TicTacToe;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Serilog;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using Games.Base.Player;
using Games.Base.Game;
using Microsoft.Extensions.DependencyInjection;
using Games.Base;

namespace WebApp.Components;
/// <summary>
/// Base class for Razor Components that are applets for games. 
/// <para>
///		This base class is concerned with providing such Components with basic methods for communicating 
///		with <see cref="GameHub{TGame, TPlayer}"/> instances as well as providing some basic variables such as <see cref="GameStates"/>
/// </para>
/// </summary>
public class GameComponent<TGame, TPlayer> : ComponentBase, IAsyncDisposable where TPlayer : PlayerModel where TGame : GameModel<TPlayer>
{
    [Inject]
    private NavigationManager NavManager { get; set; } = default!;
    [Inject]
    private IJSRuntime JS { get; set; } = default!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    private HubConnection? hubConnection;
    private TGame _game = null!;

    /// <summary>
    /// The protocol contains the most basic methods for Client <--> Hub communication.
    /// </summary>
    protected GameHubProtocol Protocol => GameHubProtocol.Singleton;

    /// <summary>
    /// Provides information about the game the user is currently in. This object should NOT be modified by Components (Clients) 
    /// and should only be touched by the server (Hubs).
    /// </summary>
    protected TGame Game
    {
        get => _game;
        set
        {
            _game = value;

            // Automatically update the TPlayer property if it exists.
            if (Self is not null)
            {
                var updatedPlayer = _game.Players.FirstOrDefault(p => p.ConnectionId == Self.ConnectionId);

                if (updatedPlayer != default(TPlayer))
                    Self = updatedPlayer;
            }
        }
    }

    /// <summary>
    /// Provides information about the user such as the Username and the Connection ID
    /// </summary>
    protected TPlayer Self { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.InvokeVoidAsync("registerListeners", DotNetObjectReference.Create(this));
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
    /// <returns>If the client is connected to the <see cref="Hub"/> </returns>
    /// <exception cref="Exception"></exception>
    protected async Task SetUpHubConnection(string hubUriName)
    {
        hubConnection = ServiceProvider.GetKeyedService<HubConnection>(hubUriName);

        if (hubConnection is null)
        {
            Log.Error("Couldn't find HubConnection with service key = \"{hubKey}\"", hubUriName);
            throw new Exception();
        }

        //	If the HubConnection is not connected, then something is WRONG!
        //		The HubConnection is started (connection established) in the GameLobby!
        if (hubConnection.State != HubConnectionState.Connected)
        {
            Log.Error("Client redirected to a game component without an active hub connection. (Connection State = {hubState})", hubConnection.State);
            throw new Exception();
        }

        // Set up receivers
        hubConnection.On(Protocol[Receivers.OnStartGame], OnStartGame);
        hubConnection.On(Protocol[Receivers.OnRestartGame], OnRestartGame);
        hubConnection.On(Protocol[Receivers.OnBeginSetup], OnBeginSetup);
        hubConnection.On<string>(Protocol[Receivers.OnFinishSetup], OnFinishSetup);
        hubConnection.On<string>(Protocol[Receivers.GetGameInstance], GetGameInstance);
        hubConnection.On<string>(Protocol[Receivers.OtherConnected], OtherConnected);
        hubConnection.On<string>(Protocol[Receivers.OtherDisconnected], OtherDisconnected);
        hubConnection.On<string, string>(Protocol[Receivers.SelfConnected], SelfConnected);

        // Let the hub know!
        await hubConnection.SendAsync(Protocol[Senders.ReadyToConnect]);
    }

    protected async Task FinishSetup<T>(T args)
    {
        if (hubConnection is null)
            return;

        // Serialize class types
        if (typeof(T).IsClass)
        {
            string argsJson = JsonConvert.SerializeObject(args);
            await hubConnection.SendAsync(Protocol[Senders.FinishSetup], argsJson);
        }

        // But do not serialize value types (primitives)
        else
        {
            await hubConnection.SendAsync(Protocol[Senders.FinishSetup], args);
        }

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
    /// Sets the <see cref="GameStates"/> to <see cref="GameStates.Playing"/> and forces UI re-render
    /// </summary>
    private async Task OnStartGame()
    {
        Log.Information("{Player} received game {Command}", Self.Name, "START");
        Game.State = GameStates.Playing;
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Calls the <see cref="GameModel.Restart"/> method and forces UI re-render
    /// </summary>
    /// <returns></returns>
    protected virtual async Task OnRestartGame()
    {
        Log.Information("{Player} received game {Command}", Self.Name, "RESTART");
        Game.Restart();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnBeginSetup()
    {
        Log.Information("Setup view set for {Player}", Self.Name);
        Game.State = GameStates.Setup;

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnFinishSetup(string gameJson)
    {
        Game = JsonConvert.DeserializeObject<TGame>(gameJson)
            ?? throw new Exception("Deserialized into NULL game instance!");

        Log.Information("{Player} received game and is transitioning from {Setup} to {Playing}", Self.Name, "Setup", "Playing");
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task GetGameInstance(string gameJson)
    {
        Game = JsonConvert.DeserializeObject<TGame>(gameJson)
            ?? throw new Exception("Deserialized into NULL game instance!");

        Log.Information("{Player} got game instance. ({size} bytes)", Self.Name, gameJson.Length);
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// This method will fire when ANOTHER player has connected to the <see cref="GameModel"/> that the client is 
    /// currently in.
    /// <para>
    /// IMPORTANT NOTE: This method does NOT send the player object to the <see cref="Hub"/>! (Using Senders.OtherPlayerConnected)
    ///		This is because the Hub can infer who the new player is thanks to the <see langword="static"/> "Games" instance
    /// </para>
    /// </summary>
    public virtual async Task OtherConnected(string json)
    {
        var player = JsonConvert.DeserializeObject<TPlayer>(json)
            ?? throw new Exception("Deserialized into NULL obhect when trying to get Player!");

        // Currently only TPlayer is sent to others with the theory that there will be reduced load.
        // Possibly change to sending the new TGame instance.
        Game.Players.Add(player);

        Log.Information("{Player} received {MethodName} method", Self.Name, nameof(OtherConnected));

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
    /// <exception cref="Exception"></exception>
    protected virtual async Task SelfConnected(string playerId, string gameJson)
    {
        var game = JsonConvert.DeserializeObject<TGame>(gameJson)
            ?? throw new Exception("Deserialized into NULL object when trying to get game object!");

        Game = game;
        Self = Game.Players.First(p => p.ConnectionId == playerId);

        Log.Information("Player \"{Name}\" succesfully connected to game \"{Game}\" ", Self.Name, Game.NameId);

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

        var playerToRemove = Game.Players.FirstOrDefault(p => p.ConnectionId == playerId)
            ?? throw new Exception("Couldn't find player to remove from gamePlayers");

        Game.Players.Remove(playerToRemove);

        Log.Information("{Player} received {MethodName} method", Self.Name, nameof(OtherDisconnected));
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

    public async Task CancelInitialization()
    {
        NavManager.NavigateTo("/applets", true);
        await DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            if (hubConnection.State != HubConnectionState.Disconnected)
                await hubConnection.StopAsync();

            await JS.InvokeVoidAsync("unregisterListeners");
            GC.SuppressFinalize(this);
        }
    }
}
