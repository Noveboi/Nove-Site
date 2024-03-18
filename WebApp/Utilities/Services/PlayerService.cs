using Games.Base;
using Games.Base.Player;
using Games.TicTacToe;

namespace WebApp.Utilities.Services;

// TODO: Add methods for handling players.
public class PlayerService 
{
	public void UpdatePlayerStats(IPlayerModel player)
	{
		switch (player.GameOverState)
		{
			case GameOverStates.Win:
				player.Stats.Wins += 1;
				break;
			case GameOverStates.Tie:
				player.Stats.Ties += 1;
				break;
			case GameOverStates.Lose:
				player.Stats.Losses += 1;
				break;
		}
	}
	public void UpdatePlayerStats(TicTacToePlayer player)
	{
		UpdatePlayerStats(player as IPlayerModel);
		player.Stats.AddSymbolToHistory(player.Symbol);
	}
}
