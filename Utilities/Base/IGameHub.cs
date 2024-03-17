namespace LearningBlazor.Utilities.Base
{
	/// <summary>
	/// Interface for hubs that inherit from GameHubBase
	/// </summary>
	public interface IGameHub<TGame, TPlayer> : IGameHubBase<TGame, TPlayer> where TGame : GameModel<TPlayer> where TPlayer : PlayerModel
	{
		/// <summary>
		/// Exposed method to client. For proper functionality you must call the 
		/// method of the same name from <see cref="IGameHubBase{TGame, TPlayer}"/>!
		/// Feel free to add any extra processing steps into this method
		/// </summary>
		Task ExposedCreateNewGame();
		/// <summary>
		/// Exposed method to client. For proper functionality you must call the 
		/// method of the same name from <see cref="IGameHubBase{TGame, TPlayer}"/>!
		/// Feel free to add any extra processing steps into this method
		/// </summary>
		Task ExposedCreatePlayer(string username);
		/// <summary>
		/// Exposed method to client. For proper functionality you must call the 
		/// method of the same name from <see cref="IGameHubBase{TGame, TPlayer}"/>!
		/// Feel free to add any extra processing steps into this method
		/// </summary>
		Task ExposedOtherPlayerDisconnected(string connectionId);
		/// <summary>
		/// Exposed method to client. For proper functionality you must call the 
		/// method of the same name from <see cref="IGameHubBase{TGame, TPlayer}"/>!
		/// Feel free to add any extra processing steps into this method
		/// </summary>
		Task ExposedClientJoinGame(string gameNameId);

		/// <summary>
		/// [CLIENT --> HUB] Players can vote to start a new game. This will happen if ALL players vote to play again.
		/// </summary>
		Task VoteToPlayAgain();
	}
}
