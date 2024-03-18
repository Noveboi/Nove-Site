using LearningBlazor.Utilities.Base.Player;
using Newtonsoft.Json;

namespace LearningBlazor.Utilities.TicTacToe;
public class TicTacToePlayer : PlayerModel
{
	public TicTacToePlayer(string connectionId, string username) : base(connectionId, username) { }

	public string Symbol { get; set; } = string.Empty;
    public bool HasRequestedPlayAgain { get; set; } = false;
}