namespace LearningBlazor.Utilities.TicTacToe;
public class TicTacToeBoard
{
    private readonly string[,] _board = new string[3,3];
    private string _symbol = string.Empty;
    private string _opponentSymbol = string.Empty;

    public event EventHandler? Lose;
    public event EventHandler? Win;
    public event EventHandler? Tie;

    public string Symbol
    {
        get => _symbol;
        set
        {
            _symbol = value;
            _opponentSymbol = _symbol == "X" ? "O" : "X";
        }
    }

    public string this[int i, int j] => _board[i, j];
    public TicTacToeBoard()
    {
        Reset();
    }

    public void Reset()
    {
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					_board[i, j] = string.Empty;
				}
			}
		}

    public void Mark(int i, int j)
    {
        if (_board[i, j] != string.Empty) return;

        _board[i, j] = _symbol;
        if (IsWinState())
        {
            Win?.Invoke(this, new EventArgs());
            return;
        }

        if (IsTieState())
            Tie?.Invoke(this, new EventArgs());
    }

    public void OpponentMark(int i, int j)
    {
        _board[i, j] = _opponentSymbol;
        if (IsWinState(forOpponent: true))
        {
            Lose?.Invoke(this, new EventArgs());
        }

			if (IsTieState())
				Tie?.Invoke(this, new EventArgs());
		}

    private bool IsTieState()
    {
        int nonEmpty = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                nonEmpty += _board[i, j] != string.Empty ? 1 : 0;
            }
        }

        return nonEmpty == 9;
    }

    private bool IsWinState(bool forOpponent = false)
    {
        string symbol = forOpponent ? _opponentSymbol : _symbol;

        // Horizontal Win
        for (int i = 0; i < 3; i++)
        {
            if (_board[i, 0] == symbol && _board[i, 1] == symbol && _board[i, 2] == symbol)
            {
                return true;
            }
        }

        // Vertical Win
        for (int j = 0; j < 3; j++)
        {
            if (_board[0, j] == symbol && _board[1, j] == symbol && _board[2, j] == symbol)
            {
                return true;
            }
        }

        // Diagonal Win
        if ((_board[0, 0] == symbol && _board[1, 1] == symbol && _board[2, 2] == symbol)
        || (_board[0, 2] == symbol && _board[1, 1] == symbol && _board[2, 0] == symbol))
        {
            return true;
        }

        return false;
    }
	}
