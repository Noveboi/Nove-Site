using Games.Base.GameModel;
using Games.Base.Player;
using Games.Base.PlayerModel;

namespace WebApp.Hubs
{
    /// <summary>
    /// Interface for hubs that inherit from GameHub
    /// </summary>
    public interface IExposeMethods<TGame, TPlayer> : IGameHubBase<TGame, TPlayer> where TGame : GameBase where TPlayer : Player
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
