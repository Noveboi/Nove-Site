namespace LearningBlazor.Utilities.Base
{
	/// <summary>
	/// Interface for hubs that inherit from GameHubBase
	/// </summary>
	/// <typeparam name="TGame"></typeparam>
	/// <typeparam name="TPlayer"></typeparam>
	public interface IGameHub<TGame, TPlayer> : IGameHubBase<TGame, TPlayer> where TGame : GameModel where TPlayer : PlayerModel
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
		Task ExposedPlayerJoinGame(string gameNameId);
	}
}
