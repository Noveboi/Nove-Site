using Games.Base.GameModel;
using Games.Base.Helpers;
using Games.TicTacToe;
using System.Reflection;

namespace Games.Base.PlayerModel;
public static class PlayerFactory
{
    // Get all GameBase subclass at runtime ONCE.
    private static IEnumerable<string> _gameTypes =
        Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.IsSubclassOf(typeof(GameBase)))
        .Select(t => t.Name);

    public static IPlayer CreatePlayer(string connectionId, string name)
    {
        Dictionary<Items, object> emptyStorage = [];
        Dictionary<string, IGameStatistics> zeroedStats = [];

        foreach (var gameType in _gameTypes)
        {
            zeroedStats[gameType] = new GameStatistics();
        }

        var instance = new Player(connectionId, name, emptyStorage, zeroedStats);
        return instance;
    }

    public static IPlayer CreatePlayer<TPlayer>(string connectionId, string name, IDictionary<string, IGameStatistics> stats)
    {
		Dictionary<Items, object> emptyStorage = [];

        var instance = new Player(connectionId, name, emptyStorage, stats);
        return instance;
	}
}
