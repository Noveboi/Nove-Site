using Games.Base.Helpers;
using Games.Base.TeamModel;
using Games.TicTacToe;
using System.Runtime.CompilerServices;

namespace Games.Base.GameModel;

/// <summary>
/// A factory pattern for creating instances of subclasses of <see cref="GameBase"/>.
/// <para>
///		The factory exposes generic methods that dynamically instantiate a subclass of <see cref="GameBase"/>. 
///		First, the given type (TGame) is recognized by the factory methods.
///		Secondly, the instance is created and any extra property/variable assignments are done.
///		Finally, the instance is returned to the caller.
/// </para>
/// </summary>
public static class GameFactory
{
	#region Exposed Factory Methods
	public static TGame CreateGame<TGame>() where TGame : GameBase
	{
		if (typeof(TGame) == typeof(TicTacToeGame))
		{
			return (CreateTicTacToeGame() as TGame)!;
		}
		else throw new Exception($"Game of type '{typeof(TGame)}' was not recognized by the factory.");

	}
	public static TGame CreateGame<TGame>(string name) where TGame : GameBase
	{
		if (typeof(TGame) == typeof(TicTacToeGame))
		{
			return (CreateTicTacToeGame(name) as TGame)!;
		}
		else throw new Exception($"Game of type '{typeof(TGame)}' was not recognized by the factory.");

	}
	public static TGame CreateGame<TGame>(IList<ITeam> teams) where TGame : GameBase
	{
		if (typeof(TGame) == typeof(TicTacToeGame))
		{
			return (CreateTicTacToeGame(teams) as TGame)!;
		}
		else throw new Exception($"Game of type '{typeof(TGame)}' was not recognized by the factory.");

	}
	public static TGame CreateGame<TGame>(string name, IList<ITeam> teams) where TGame : GameBase
	{
		if (typeof(TGame) == typeof(TicTacToeGame))
		{
			return (CreateTicTacToeGame(name, teams) as TGame)!;
		}
		else throw new Exception($"Game of type '{typeof(TGame)}' was not recognized by the factory.");
	}
	#endregion
	#region TicTacToe
	private static TicTacToeGame CreateTicTacToeGame()
	{
		var turnManager = new TurnManager();
		var emptyTeamList = new List<ITeam>();

		return new TicTacToeGame(turnManager, emptyTeamList);
	}
	private static TicTacToeGame CreateTicTacToeGame(string name)
	{
		var turnManager = new TurnManager();
		var emptyTeamList = new List<ITeam>();

		return new TicTacToeGame(turnManager, name, emptyTeamList);
	}
	private static TicTacToeGame CreateTicTacToeGame(IList<ITeam> teams)
	{
		var turnManager = new TurnManager();
		return new TicTacToeGame(turnManager, teams);
	}
	private static TicTacToeGame CreateTicTacToeGame(string name, IList<ITeam> teams)
	{
		var instance = CreateTicTacToeGame(teams);
		instance.Title = name;
		return instance;
	}
	#endregion
}
