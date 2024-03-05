using Microsoft.AspNetCore.SignalR;
namespace LearningBlazor.Utilities.Base;

/// <summary>
/// Implemented by all hubs that inherit from <see cref="GameHub{T}"/>
/// </summary>
public interface IGameHub 
{
	/// <summary>
	/// [CLIENT --> HUB] Client orders the <see cref="GameHub{T}"/> to create a new game in the <see cref="GameModel"/> list 
	/// and notify all other <see cref="GameHub{T}"/> clients. Furthermore, the 'Game' value is set in the <see cref="HubCallerContext.Items"/>
	/// storage
	/// </summary>
	Task CreateNewGame();

	/// <summary>
	/// [CLIENT --> HUB] Called when someone else disconnects from the same game as the <see cref="GameHub{T}"/> client
	/// </summary>
	Task HandleOtherPlayerDisconnect();

	/// <summary>
	/// [CLIENT --> HUB] The <see cref="GameHub{T}"/> adds a new instance of <see cref="PlayerModel"/> with <paramref name="username"/>
	/// to the <see cref="Dictionary{TKey, TValue}"/>
	/// </summary>
	/// <param name="username">The name of the player that is displayed instead on the Connection ID</param>
	/// <returns></returns>
	Task CreatePlayer(string username);

	/// <summary>
	/// [CLIENT --> HUB] When a user clicks on an available <see cref="GameModel"/> in the <see cref="Components.Applets.GameLobby"/>,
	/// the user's client sends the <see cref="GameModel.NameId"/> property to the <see cref="GameHub{TGame}"/> and the player is assigned 
	/// to the corresponding <see cref="GameModel"/>
	/// </summary>
	/// <param name="gameNameId"></param>
	/// <returns></returns>
	Task ClientJoinGame(string gameNameId);

}
