using Games.Base.PlayerModel;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Games.Base.TeamModel;

/// <summary>
/// A list-like collection of players. Has some extra functionality regarding game logic.
/// </summary>
/// <typeparam name="TPlayer"></typeparam>
public class Team : ITeam
{
	public Team(string name, IEnumerable<IPlayer> players)
	{
		Name = name;
		_players = players.ToList();
	}

	[JsonRequired]
	private readonly IList<IPlayer> _players;
	public IPlayer this[int index] => _players[index];
	public IPlayer? this[string connectionId]
	{
		get => _players.FirstOrDefault(p => p.ConnectionId == connectionId);
	}


	/// <summary>
	/// This method is invoked when the GameBase is OVER.
	/// </summary>
	private Action<IPlayer> _updatePlayerStatsAfterGame;

	// Basic list manipulation
	public void AddPlayer(IPlayer player)
		=> _players.Add(player);
	public void RemovePlayer(IPlayer player)
		=> _players.Remove(player);

	// Basic list inspection
	public int Count => _players.Count;

	public string Name { get; set; }


	// Call when the game is over
	public void OnGameOver(GameOverStates gameOverState)
	{
		foreach (var player in _players)
		{
			if (_updatePlayerStatsAfterGame is not null)
			{
				_updatePlayerStatsAfterGame(player);
			}
		}
	}

	// Implement IEnumerable
	public IEnumerator<IPlayer> GetEnumerator() => _players.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _players.GetEnumerator();
}
