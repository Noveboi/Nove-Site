using LearningBlazor.Utilities.Base;
using Newtonsoft.Json;

namespace LearningBlazor.Utilities.TicTacToe;
public class TicTacToeGame : GameModel
{
    public TicTacToeGame() : base() { }
    public TicTacToeGame(string name) : base(name) { }
    public TicTacToeGame(TicTacToePlayer player1) : base(player1) { }
	public TicTacToeGame(TicTacToePlayer player1, TicTacToePlayer player2) : base(player1, player2) { }
	public TicTacToeGame(string name, TicTacToePlayer player1) : base(name, player1) { }
	public TicTacToeGame(string name, TicTacToePlayer player1, TicTacToePlayer player2) : base(name, player1, player2) { }

    public override int PlayerCapacity => 2;

    [JsonIgnore]
    public int PlayAgainRequests { get; private set; } = 0;

	public bool RequestPlayAgain()
    {
        PlayAgainRequests++;
        if (PlayAgainRequests == 2)
        {
            PlayAgainRequests = 0;
            return true;
        }
        return false;
    }
}
