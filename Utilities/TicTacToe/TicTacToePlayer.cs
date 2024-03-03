
namespace LearningBlazor.Utilities.TicTacToe;
public class TicTacToePlayer(string connectionId, string username) : IPlayer
{
	public string Id { get; } = connectionId;
	public string Name { get; } = username;
	public string Symbol { get; set; } = string.Empty;
    public bool HasRequestedPlayAgain { get; set; } = false;
}