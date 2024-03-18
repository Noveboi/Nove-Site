using Newtonsoft.Json;

namespace Games.TicTacToe;
public class TicTacToeBoardModel
{
    [JsonRequired]
    private readonly string[] _board = new string[9];
    public string this[int i, int j] => _board[3*i + j];

    public TicTacToeBoardModel()
    {
        Reset();
    }

    public void Reset()
    {
		for (int i = 0; i < 9; i++)
		{
            _board[i] = string.Empty;
		}
	}

    public void Mark(string symbol, int i, int j)
    {
        if (this[i, j] != string.Empty) return;

        _board[3*i + j] = symbol;
    }
}
