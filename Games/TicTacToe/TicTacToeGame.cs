using Games.Base;
using Games.Base.GameModel;
using Games.Base.Helpers;
using Games.Base.PlayerModel;
using Games.Base.TeamModel;

namespace Games.TicTacToe;
public class TicTacToeGame : GameBase, ICanWin, ITurnBased
{
	#region Constructors
	internal TicTacToeGame(ITurnManager turnManager, IEnumerable<ITeam> teams) : base(teams)
	{
		TurnManager = turnManager;
		UniqueId = new(typeof(TicTacToeGame));
	}

	internal TicTacToeGame(ITurnManager turnManager, string name, IEnumerable<ITeam> teams) : base(name, teams)
	{
		TurnManager = turnManager;
		UniqueId = new(typeof(TicTacToeGame));
	}
	#endregion

	public ITurnManager TurnManager { get; }
	public override int PlayerCapacity => 2;
	public override UniqueGameId UniqueId { get; }
	public TicTacToeBoardModel Board { get; set; } = new();

	public IPlayer TurnPlayer => TurnManager.PlayingTeam[0];

	public event EventHandler<GameOverEventArgs>? GameOver;

	public override void Setup()
	{
		State = GameStates.Setup;
	}
	public override void Start()
	{
		TurnManager.Activate(Teams);
		State = GameStates.Playing;
	}
	public override void Restart(bool toSetup = false)
	{
		Board.Reset();
		State = toSetup ? GameStates.Setup : GameStates.Playing;
	}
	public override void HandleEndByDisconnect()
	{
		// TODO: Invoke GameOver
	}

	public override void HandleEndByWinOrTie()
	{
		// TODO: Invoke GameOver
	}

	public bool IsTieState()
	{
		int nonEmpty = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				nonEmpty += Board[i, j] != string.Empty ? 1 : 0;
			}
		}

		return nonEmpty == 9;
	}

	public bool IsWinStateFor(ITeam team)
	{
		var player = team[0];
		string symbol = (string)player.Storage[Items.Symbol];

		// Horizontal Win
		for (int i = 0; i < 3; i++)
		{
			if (Board[i, 0] == symbol && Board[i, 1] == symbol && Board[i, 2] == symbol)
			{
				return true;
			}
		}

		// Vertical Win
		for (int j = 0; j < 3; j++)
		{
			if (Board[0, j] == symbol && Board[1, j] == symbol && Board[2, j] == symbol)
			{
				return true;
			}
		}

		// Diagonal Win
		if ((Board[0, 0] == symbol && Board[1, 1] == symbol && Board[2, 2] == symbol)
		|| (Board[0, 2] == symbol && Board[1, 1] == symbol && Board[2, 0] == symbol))
		{
			return true;
		}

		return false;
	}

	public void Mark(int i, int j)
	{
		string symbol = (string)TurnPlayer.Storage[Items.Symbol];

		Board.Mark(symbol, i, j);
		TurnManager.NextTurn();
	}
}
