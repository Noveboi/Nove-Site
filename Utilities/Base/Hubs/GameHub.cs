using LearningBlazor.Utilities.Base.Components;
using LearningBlazor.Utilities.Base.Game;
using LearningBlazor.Utilities.Base.Player;
using LearningBlazor.Utilities.TicTacToe;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace LearningBlazor.Utilities.Base.Hubs;

/// <summary>
/// Base class for any SignalR <see cref="Hub"/> that is concerned with game applets.
/// This class provides basic methods for communicating with <see cref="Hub"/> Clients and 
/// includes but is not limited to methods such as:
/// <list type="bullet">
///		<item> Sending the <see cref="List{T}"/> to the connected client's lobby </item>
///		<item> Sending a notification if a client disconnects from the hub and handling said notification</item>
///		<item> Creating a new <typeparamref name="TGame"/> if a client requests it </item>
///		<item> </item>
/// </list>
/// <typeparamref name="TGame"/> is the type of <see cref="GameModel"/> associated with each class that inherits from <see cref="GameHub{T,T}"/>
/// <typeparamref name="TPlayer"/> is the type of <see cref="PlayerModel"/> associated with each class that inherits from <see cref="GameHub{TGame, TPlayer}"/>
/// </summary>
public class GameHub<TGame, TPlayer> : Hub, IGameHubBase<TGame, TPlayer> where TGame : GameModel<TPlayer> where TPlayer : PlayerModel
{
    #region Fields

    // HubCallerContext.Items Keys
    protected enum ItemKeys
    {
        Player,
        Game,
        Opponent,   // For 2-player games
        PlayAgainVotes
    }

    #endregion
    #region Hub Properties
    /* 
		The properties 'Game' and 'Player' use the Context.Items collection for their backing fields.
		This is because the GameHub instance could be disposed and reinitialized. 
		If we were initializing these properties "normally" then they would also be reinitialized in the above case.
		Storing the states of these objects inside Context.Items avoids this problem!
	 */
    public TGame Game
    {
        get
        {
            TGame? g = Context.Items[ItemKeys.Game] as TGame
                ?? throw new Exception("Trying to access a NULL game instance, please access the Game property after the player creates/joins a game!");
            return g;
        }
        set => Context.Items[ItemKeys.Game] = value;
    }
    public TPlayer Player
    {
        get
        {
            TPlayer? p = Context.Items[ItemKeys.Player] as TPlayer
                ?? throw new Exception("Trying to access a NULL player instance, please access the Player property after the client sends username info to the hub!");
            return p;
        }
        set => Context.Items[ItemKeys.Player] = value;
    }

    protected static GameHubProtocol Protocol => GameHubProtocol.Singleton;
    #endregion
    #region Methods

    /// <summary>
    /// Wrapper method for <see cref="IHubClients.Client(string)"/> SendAsync method. Used in certain scenarios
    ///	<para>
    ///		This method broadcasts to all <see cref="GameModel.Players"/> clients to call the <paramref name="methodName"/> method in
    ///		their own Component.
    /// </para>
    /// </summary>
    private async Task NotifyGamePlayers(string methodName, bool exceptSelf = false)
    {
        var gamePlayers = Game.Players
            .Select(p => p.ConnectionId);

        if (exceptSelf)
            gamePlayers = gamePlayers.Except([Context.ConnectionId]);

        await Clients.Clients(gamePlayers).SendAsync(methodName);
    }

    /// <summary>
    /// Wrapper method for <see cref="IHubClients.Client(string)"/> SendAsync method. Used in certain scenarios
    ///	<para>
    ///		This method broadcasts to all <see cref="GameModel.Players"/> clients to call the <paramref name="methodName"/> method 
	///		with the passed argument <paramref name="arg1"/> in their own Component.
    /// </para>
    /// </summary>
    private async Task NotifyGamePlayers(string methodName, object? arg1, bool exceptSelf = false)
    {
        var gamePlayers = Game.Players
            .Select(p => p.ConnectionId);

        if (exceptSelf)
            gamePlayers = gamePlayers.Except([Context.ConnectionId]);

        await Clients.Clients(gamePlayers).SendAsync(methodName, arg1);
    }

    // Overrides the Hub method for handling client disconnects. This however does NOT catch instances
    // where the client CLOSES their tab/browser
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Game.Players.Remove(Player);

        await NotifyGamePlayers(Protocol[Receivers.OtherDisconnected], Player.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task OnBrowserClose() =>
        await OnDisconnectedAsync(new Exception("Player disconnected by means of closing browser or tab."));

    // Send to LOBBY
    public async Task SendGameListToClient(List<TGame> games)
    {
        string json = JsonConvert.SerializeObject(games);
        await Clients.Caller.SendAsync(Protocol[Receivers.GetGameList], json);
    }

    // Sent from LOBBY
    public Task CreatePlayer(Dictionary<string, TPlayer> playerDict, string username)
    {
        var player = PlayerFactory.CreatePlayer<TPlayer>(Context.ConnectionId, username);
        playerDict[Context.ConnectionId] = player;
        Player = player;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sent from <see cref="GameComponent{TGame, TPlayer}"/> when the client has finished setting up the HubConnection.
    /// </summary>
    /// <returns></returns>
    public virtual async Task ReadyToConnect()
    {
        Game.Players.Add(Player);
        await NotifyOfClientConnection();

        var gameJson = JsonConvert.SerializeObject(Game);

        // TODO: Inspect this, this is most likely completely wrong!!!
        // TODO: Inspect this, this is most likely completely wrong!!!
        // TODO: Inspect this, this is most likely completely wrong!!!
        // TODO: Inspect this, this is most likely completely wrong!!!
        // TODO: Inspect this, this is most likely completely wrong!!!
        if (Game.Players.Count == 1)
            await Clients.Others.SendAsync(Protocol[Receivers.UpdateGameList], gameJson);
    }

    public Task ClientJoinGame(List<TGame> games, string gameNameId)
    {
        var game = games.FirstOrDefault(g => g.NameId == gameNameId)
            ?? throw new Exception("Game not found!");

        Game = game;
        return Task.CompletedTask;
    }

    // For now, does not send Game to Clients in order to avoid sending a lot of data. This could be an issue in the future. 
    public async Task StartGame()
    {
        Game.State = GameStates.Playing;
        await NotifyGamePlayers(Protocol[Receivers.OnStartGame]);
    }

    // For now, does not send Game to Clients in order to avoid sending a lot of data. This could be an issue in the future. 
    public async Task RestartGame()
    {
        Game.Restart();
        await NotifyGamePlayers(Protocol[Receivers.OnRestartGame]);
    }

    public async Task BeginSetup()
    {
        Game.State = GameStates.Setup;
        await NotifyGamePlayers(Protocol[Receivers.OnBeginSetup]);
    }

    /// <summary>
    /// Transition the game state from <see cref="GameStates.Setup"/> to <see cref="GameStates.Playing"/> and send the Game object to players
    /// 
    /// <para>
    ///		<paramref name="argsJson"/> is an <see cref="object"/> of any <see cref="Type"/> that deriving classes
    ///		can use to do any extra steps (e.g: Set player symbols in TicTacToe)
    /// </para>
    /// </summary>
    public virtual async Task FinishSetup(string argsJson)
    {
        Game.State = GameStates.Playing;

        string gameJson = JsonConvert.SerializeObject(Game);

        await NotifyGamePlayers(Protocol[Receivers.OnFinishSetup], gameJson);
    }

    public async Task NotifyOfClientConnection()
    {
        string playerJson = JsonConvert.SerializeObject(Player);
        string gameJson = JsonConvert.SerializeObject(Game);

        await Clients.Caller.SendAsync(Protocol[Receivers.SelfConnected], Player.ConnectionId, gameJson);
        await NotifyGamePlayers(Protocol[Receivers.OtherConnected], playerJson, exceptSelf: true);
    }

    public virtual Task OtherPlayerConnected()
        => Task.CompletedTask;


    // Sent from LOBBY
    public Task CreateNewGame(List<TGame> games)
    {
        var game = Activator.CreateInstance(typeof(TGame)) as TGame
            ?? throw new Exception($"Couldn't instantiate {nameof(TGame)} object");
        games.Add(game);
        Game = game;

        return Task.CompletedTask;
    }

    public async Task SendGameToPlayers()
    {
        string gameJson = JsonConvert.SerializeObject(Game);
        await NotifyGamePlayers(Protocol[Receivers.GetGameInstance], gameJson);
    }

    public Task OtherPlayerDisconnected(Dictionary<string, TPlayer> playerDict, string connectionId)
    {
        return Task.CompletedTask;
    }
    #endregion
}