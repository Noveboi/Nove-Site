using LearningBlazor.Utilities.Base.Game;
using Newtonsoft.Json;

namespace LearningBlazor.Utilities.TicTacToe;
public class TicTacToeGame : GameModel<TicTacToePlayer>
{
    public TicTacToeGame() : base() { }
    public TicTacToeGame(string name) : base(name) { }
    public TicTacToeGame(IEnumerable<TicTacToePlayer> playerCollection) : base(playerCollection) { }
	public TicTacToeGame(string name, IEnumerable<TicTacToePlayer> playerCollection) : base(name, playerCollection) { }

    public override int PlayerCapacity => 2;

    public TicTacToeBoardModel Board { get; set; } = new();

	public override void Restart()
	{
		base.Restart();
		Board.Reset();
	}

	/// <summary>
	/// Swap player turns
	/// </summary>
	public void NextTurn()
	{
		bool player1Turn = Players[0].HasTurn;
		Players[0].HasTurn = !player1Turn;
		Players[1].HasTurn = player1Turn;
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

	public bool IsWinStateFor(TicTacToePlayer player)
	{
		// Horizontal Win
		for (int i = 0; i < 3; i++)
		{
			if (Board[i, 0] == player.Symbol && Board[i, 1] == player.Symbol && Board[i, 2] == player.Symbol)
			{
				return true;
			}
		}

		// Vertical Win
		for (int j = 0; j < 3; j++)
		{
			if (Board[0, j] == player.Symbol && Board[1, j] == player.Symbol && Board[2, j] == player.Symbol)
			{
				return true;
			}
		}

		// Diagonal Win
		if ((Board[0, 0] == player.Symbol && Board[1, 1] == player.Symbol && Board[2, 2] == player.Symbol)
		|| (Board[0, 2] == player.Symbol && Board[1, 1] == player.Symbol && Board[2, 0] == player.Symbol))
		{
			return true;
		}

		return false;
	}
}
