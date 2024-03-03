namespace LearningBlazor.Utilities.TicTacToe;
public class TicTacToeGame : IGame
{
    public TicTacToeGame(TicTacToePlayer player1) 
    {
        Players = [player1];
    }

    public TicTacToeGame(TicTacToePlayer player1, TicTacToePlayer player2)
    {
        Players = [player1, player2];
    }

    public List<IPlayer> Players { get; }
    public int PlayAgainRequests { get; private set; } = 0;

    public void RemoveOpponentOf(TicTacToePlayer player)
    {
        var opponent = Players[0] == player ? Players[1] : Players[0];
        Players.Remove(opponent);
    }
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
