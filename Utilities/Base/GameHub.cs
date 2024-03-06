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
///		<item> Creating a new <see cref="Game"/> if a client requests it </item>
///		<item> </item>
/// </list>
/// <typeparamref name="TGame"/> is the type of game associated with each class that inherits from <see cref="GameHub{TGame}"/>
/// </summary>
public class GameHub<TGame, TPlayer> : Hub where TGame : GameModel where TPlayer : PlayerModel
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
	protected TGame Game
	{
		get
		{
			TGame? g = Context.Items[ItemKeys.Game] as TGame
				?? throw new Exception("Trying to access a NULL game instance, please access the Game property after the player creates/joins a game!");
			return g;
		}
		set => Context.Items[ItemKeys.Game] = value;
	}
	protected TPlayer Player
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

	private async Task NotifyGamePlayers(string methodName)
	{
		var gamePlayers = Game.Players.Select(p => p.Id);
		await Clients.Clients(gamePlayers).SendAsync(methodName);
	}

	private async Task NotifyGamePlayers(string methodName, object? arg1)
	{
		var gamePlayers = Game.Players.Select(p => p.Id);
		await Clients.Clients(gamePlayers).SendAsync(methodName, arg1);
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		Game.Players.Remove(Player);

		await NotifyGamePlayers(GameComponent.RECEIVERS_PLAYER_DISCONNECTED, Player.Id);
		await base.OnDisconnectedAsync(exception);
	}

	public async Task ForceDisconnectOnBrowserClose() =>
		await OnDisconnectedAsync(new Exception("Player disconnected by means of closing browser or tab."));

	/// <summary>
	/// Serializes the 'Games' list into JSON and sends it to the client associated with the hub
	/// </summary>
	protected async Task SendGameListToClient(List<TGame> games) 
	{
		string json = JsonConvert.SerializeObject(games);
		await Clients.Caller.SendAsync(method: "GetGameList", arg1: json);
	}

	protected async Task NotifyGameStart()
	{
		await NotifyGamePlayers(GameComponent.RECEIVERS_BEGIN_GAME); 
	}

	protected async Task NotifyPlayersOfConnect()
	{
		string playerJson = JsonConvert.SerializeObject(Player)
			?? throw new Exception("nuh uh!");
		string gamePlayersJson = JsonConvert.SerializeObject(Game.Players)
			?? throw new Exception("nuh uh uh!");

		await Clients.Caller.SendAsync(GameComponent.RECEIVERS_CLIENT_CONNECTED, gamePlayersJson);
		await NotifyGamePlayers(GameComponent.RECEIVERS_PLAYER_CONNECTED, playerJson);
	}
	#endregion
}