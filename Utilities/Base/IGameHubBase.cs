using Microsoft.AspNetCore.SignalR;
namespace LearningBlazor.Utilities.Base;

/// <summary>
/// Implemented by all hubs that inherit from <see cref="GameHubBase{T,T}"/>
/// </summary>
public interface IGameHubBase<TGame, TPlayer> where TGame : GameModel where TPlayer : PlayerModel
{
	/// <summary>
	/// The instance of the match the <typeparamref name="TPlayer"/> is currently in.
	/// </summary>
	TGame Game { get; set; }
	/// <summary>
	/// The instance of the player, contains information such as Connection ID and Username
	/// </summary>
	TPlayer Player { get; set; }

	/// <summary>
	/// [CLIENT --> HUB] Client orders the <see cref="GameHubBase{T,T}"/> to create a new game in the <see cref="GameModel"/> list 
	/// and notify all other <see cref="GameHub{T}"/> clients. Furthermore, the 'Game' value is set in the <see cref="HubCallerContext.Items"/>
	/// storage
	/// </summary>
	/// <param name="gameList">
	///		The STATIC list in an implementation of <see cref="IGameHub{TGame, TPlayer}"/> 
	///		containing all the <typeparamref name="TGame"/> instances in that Hub
	/// </param>
	Task CreateNewGame(List<TGame> gameList);

	/// <summary>
	/// [CLIENT --> HUB] The <see cref="GameHubBase{T,T}"/> adds a new instance of a <typeparamref name="TPlayer"/> with Name = <paramref name="username"/>
	/// to the <paramref name="playerDict"/>
	/// </summary>
	/// <param name="username">The name of the player that is displayed instead on the Connection ID</param>
	/// <param name="playerDict">
	///		The STATIC dictionary in an implementation of <see cref="IGameHub{TGame, TPlayer}"/> 
	///		containing all the <typeparamref name="TPlayer"/> instances in that Hub
	///	</param>
	/// <returns></returns>
	Task CreatePlayer(Dictionary<string, TPlayer> playerDict, string username);

	/// <summary>
	/// [CLIENT --> HUB] Called when someone else disconnects from the same game as the <see cref="GameHubBase{T, T}"/> client
	/// </summary>
	Task OtherPlayerDisconnected(Dictionary<string, TPlayer> playerDict, string connectionId);

	/// <summary>
	/// [CLIENT --> HUB] When a user clicks on an available <see cref="GameModel"/> in the <see cref="Components.Applets.GameLobby"/>,
	/// the user's client sends the <see cref="GameModel.NameId"/> property to the <see cref="GameHubBase{TGame, TPlayer}"/> and the player is assigned 
	/// to the corresponding <typeparamref name="TGame"/>
	/// </summary>
	/// <param name="gameNameId">The NameId that identifies the specific <typeparamref name="TGame"/></param>
	/// <param name="gameList">
	///		The STATIC list in an implementation of <see cref="IGameHub{TGame, TPlayer}"/> 
	///		containing all the <typeparamref name="TGame"/> instances in that Hub
	/// </param>
	/// <returns></returns>
	Task ClientJoinGame(List<TGame> gameList, string gameNameId);

	/// <summary>
	/// Detect tab/browser close through JavaScript and propagate the event to this method to force the 
	/// <see cref="Hub.OnDisconnectedAsync(Exception?)"/> method
	/// </summary>
	/// <returns></returns>
	Task OnBrowserClose();

	/// <summary>
	/// [HUB --> CLIENT] Send the Games list to the caller as a JSON string
	/// </summary>
	/// <returns></returns>
	Task SendGameListToClient(List<TGame> games);
	/// <summary>
	/// [HUB --> CLIENT] Broadcasts message to ALL clients in <see cref="Game"/> indicating they should set their 
	/// gameState to <see cref="GameStates.Playing"/>
	/// </summary>
	/// <returns></returns>
	Task NotifyGameStart();

	/// <summary>
	/// Broadcasts the following messages:
	/// <list type="bullet">
	///		<item> [TO ALL PLAYERS EXCEPT CALLER] -> JSON string containing the caller's <typeparamref name="TPlayer"/> information </item>
	///		<item> [TO CALLER ONLY] -> JSON string of the entire <see cref="Game.Players"/> list </item>
	/// </list>
	/// </summary>
	Task NotifyOfClientConnection();
}
