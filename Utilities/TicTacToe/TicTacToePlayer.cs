using LearningBlazor.Utilities.Base.Player;
using Newtonsoft.Json;

namespace LearningBlazor.Utilities.TicTacToe;
public class TicTacToePlayer : PlayerModel
{
	public TicTacToePlayer(string connectionId, string username, TicTacToePlayerStats stats) : base(connectionId, username, stats) 
	{
		Stats = stats;
	}

	public string Symbol { get; set; } = string.Empty;
	public new TicTacToePlayerStats Stats { get; set; }
}