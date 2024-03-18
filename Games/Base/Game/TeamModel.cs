using Games.Base.Player;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Games.Base.Game;

public class TeamModel<TPlayer> : IEnumerable<TPlayer> where TPlayer : PlayerModel
{
	[JsonConstructor]
	public TeamModel(Action<TPlayer> statsUpdateAfterGame)
	{
		_updatePlayerStatsAfterGame = statsUpdateAfterGame;
	}

	[JsonRequired]
	private readonly IList<TPlayer> _players = [];
	public TPlayer this[int index] => _players[index];

	/// <summary>
	/// This method is invoked when the Game is OVER.
	/// </summary>
	[JsonRequired]
	private Action<TPlayer> _updatePlayerStatsAfterGame;

	// Basic list manipulation
	public void AddPlayer(TPlayer player)
		=> _players.Add(player);
	public void RemovePlayer(TPlayer player)
		=> _players.Remove(player);

	// Call when the game is over
	public void OnGameOver(GameOverStates gameOverState)
	{
		foreach (var player in _players)
		{
			player.GameOverState = gameOverState;
			_updatePlayerStatsAfterGame(player);
		}
	}

	// Implement IEnumerable
	public IEnumerator<TPlayer> GetEnumerator() => _players.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _players.GetEnumerator();
}
