﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace LearningBlazor.Utilities.Base;

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
/// <typeparamref name="TGame"/> is the type of <see cref="GameModel"/> associated with each class that inherits from <see cref="GameHubBase{T,T}"/>
/// <typeparamref name="TPlayer"/> is the type of <see cref="PlayerModel"/> associated with each class that inherits from <see cref="GameHubBase{TGame, TPlayer}"/>
/// </summary>
public class GameHubBase<TGame, TPlayer> : Hub, IGameHubBase<TGame, TPlayer> where TGame : GameModel where TPlayer : PlayerModel
{
	#region Fields

	public const string SENDERS_PLAYER_BROWSER_CLOSE = nameof(ForceDisconnectOnBrowserClose);

	// HubCallerContext.Items Keys
	protected enum ItemKeys
	{
		Player,
		Game,
		UserPlaying,
		Opponent	// For 2-player games
	}
	
	#endregion
	#region Hub Properties
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
	protected bool IsUserPlaying
	{
		get
		{
			object usrPlaying = Context.Items[ItemKeys.UserPlaying] ?? false;
			return (bool)usrPlaying;
		}
		set => Context.Items[ItemKeys.UserPlaying] = value;
	}
	#endregion
	#region Server-Only Methods

	/// <summary>
	/// Wrapper method for <see cref="IHubClients.Client(string)"/> SendAsync method. Used in certain scenarios
	///	<para>
	///		This method broadcasts to all <see cref="GameModel.Players"/> clients to call the <paramref name="methodName"/> method in
	///		their own Component.
	/// </para>
	/// </summary>
	private async Task NotifyGamePlayers(string methodName)
	{
		var gamePlayers = Game.Players
			.Select(p => p.Id);

		await Clients.Clients(gamePlayers).SendAsync(methodName);
	}

    /// <summary>
    /// Wrapper method for <see cref="IHubClients.Client(string)"/> SendAsync method. Used in certain scenarios
    ///	<para>
    ///		This method broadcasts to all <see cref="GameModel.Players"/> clients to call the <paramref name="methodName"/> method 
	///		with the passed argument <paramref name="arg1"/> in their own Component.
    /// </para>
    /// </summary>
    private async Task NotifyGamePlayers(string methodName, object? arg1)
	{
		var gamePlayers = Game.Players
			.Select(p => p.Id);

		await Clients.Clients(gamePlayers).SendAsync(methodName, arg1);
	}

	// Overrides the Hub method for handling client disconnects. This however does NOT catch instances
	// where the client CLOSES their tab/browser
	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		Game.Players.Remove(Player);

		await NotifyGamePlayers(GameComponent<TPlayer>.RECEIVERS_OTHER_DISCONNECTED, Player.Id);
		await base.OnDisconnectedAsync(exception);
	}

	public async Task ForceDisconnectOnBrowserClose() =>
		await OnDisconnectedAsync(new Exception("Player disconnected by means of closing browser or tab."));

	public async Task SendGameListToClient(List<TGame> games) 
	{
		string json = JsonConvert.SerializeObject(games);
		await Clients.Caller.SendAsync(method: "GetGameList", arg1: json);
	}

	public async Task CreatePlayer(Dictionary<string, TPlayer> playerDict, string username)
	{
		var player = Activator.CreateInstance(typeof(TPlayer), Context.ConnectionId, username) as TPlayer
			?? throw new Exception($"Couldn't instantiate player of type {nameof(TPlayer)}");
		playerDict[Context.ConnectionId] = player;
		Player = player;

		var playerJson = JsonConvert.SerializeObject(Player);

		await Clients.Caller.SendAsync(GameComponent<TPlayer>.RECEIVERS_GET_PLAYER_MODEL, playerJson);
	}

	public async Task PlayerJoinGame(List<TGame> games, string gameNameId)
	{
		var game = games.FirstOrDefault(g => g.NameId == gameNameId)
			?? throw new Exception("Game not found!");

		Game = game;
		await NotifyOthersOfNewConnection();
		game.Players.Add(Player);
	}

	public async Task NotifyGameStart() =>
		await NotifyGamePlayers(GameComponent<TPlayer>.RECEIVERS_BEGIN_GAME);

    public async Task NotifyOthersOfNewConnection()
	{
		string playerJson = JsonConvert.SerializeObject(Player);
		string gamePlayersJson = JsonConvert.SerializeObject(Game.Players);

		await Clients.Caller.SendAsync(GameComponent<TPlayer>.RECEIVERS_SELF_CONNECTED, gamePlayersJson);
		await NotifyGamePlayers(GameComponent<TPlayer>.RECEIVERS_OTHER_CONNECTED, playerJson);
	}

	public async Task CreateNewGame(List<TGame> games)
	{
		var game = Activator.CreateInstance(typeof(TGame), Player) as TGame
			?? throw new Exception($"Couldn't instantiate {nameof(TGame)} object");
		games.Add(game);
		Game = game;

		string gameJson = JsonConvert.SerializeObject(game);

		// For anyone else in this hub, send the updated game list 
		await Clients.Others.SendAsync(GameComponent<TPlayer>.RECEIVERS_UPDATE_GAME_LIST, gameJson);
	}

	public Task OtherPlayerDisconnected(Dictionary<string, TPlayer> playerDict, string connectionId)
	{
		var player = playerDict[connectionId];
		Game.Players.Remove(player);

		return Task.CompletedTask;
	}
	#endregion
}