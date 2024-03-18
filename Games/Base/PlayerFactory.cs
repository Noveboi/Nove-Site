using Games.Base.Player;
using Games.TicTacToe;

namespace Games.Base;
public static class PlayerFactory
{
    /// <summary>
    /// Instantiates a new <see cref="PlayerModel"/> object with empty stats
    /// </summary>
    public static PlayerModel CreateBasePlayer(string connectionId, string name)
    {
        return new PlayerModel(connectionId, name, new PlayerStats());
    }

    /// <summary>
    /// Instantiates a new <see cref="PlayerModel"/> object with the given <paramref name="stats"/>
    /// </summary>
    public static PlayerModel CreateBasePlayer(string connectionId, string name, PlayerStats stats)
    {
        return new PlayerModel(connectionId, name, stats);
    }

	public static TPlayer CreatePlayer<TPlayer>(string connectionId, string name) where TPlayer : PlayerModel
    {
        if (typeof(TPlayer) == typeof(TicTacToePlayer))
        {
            var emptyStats = new TicTacToePlayerStats();
            TPlayer? instance = Activator.CreateInstance(typeof(TicTacToePlayer), connectionId, name, emptyStats) as TPlayer 
                ?? throw new Exception("Activator couldn't instantiate TPlayer object.");

			return instance;
        }
        else
        {
            throw new NotSupportedException($"Type '{typeof(TPlayer)}' is not recognized by the Factory.");
        }
    }

	public static TPlayer CreatePlayer<TPlayer, TStats>(string connectionId, string name, TStats stats) 
        where TPlayer : PlayerModel where TStats : PlayerStats
	{
		if (typeof(TPlayer) == typeof(TicTacToePlayer))
		{
			TPlayer? instance = Activator.CreateInstance(typeof(TicTacToePlayer), connectionId, name, stats) as TPlayer
				?? throw new Exception("Activator couldn't instantiate TPlayer object.");

			return instance;
		}
		else
		{
			throw new NotSupportedException($"Type '{typeof(TPlayer)}' is not recognized by the Factory.");
		}
	}
}
