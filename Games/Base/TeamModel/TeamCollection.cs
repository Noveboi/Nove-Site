using Games.Base.PlayerModel;
using Games.Base.TeamModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Base.Team;

/// <summary>
/// A dictionary-like represantation of ITeam instances. 
/// </summary>
public class TeamCollection : IEnumerable
{
	public TeamCollection(IEnumerable<ITeam> teams)
	{
		_teams = new List<ITeam>();
		_players = new Dictionary<string, IPlayer>();
		foreach (var team in teams)
		{
			Add(team);
		}
	}

	private readonly IList<ITeam> _teams;
	private readonly IDictionary<string, IPlayer> _players;

	/// <summary>
	/// Get an <see cref="ITeam"/> instance based on the team's name.
	/// </summary>
	/// <param name="teamName"></param>
	/// <returns></returns>
	public ITeam this[string teamName]
	{
		get => _teams.First(t => t.Name == teamName);
	}

	public ITeam this[int index]
	{
		get => _teams[index];
	}

	public int Count => _teams.Count;

	public IPlayer FindPlayer(string connectionId) => _players[connectionId];

	public void Add(ITeam team)
	{
		_teams.Add(team);

		// Update player list
		foreach (var player in team)
		{
			_players[player.ConnectionId] = player;
		}
	}
	public bool RemoveTeam(ITeam team)
	{
		foreach (var player in team)
		{
			_players.Remove(player.ConnectionId);
		}

		return _teams.Remove(team);
	}

	public IEnumerator GetEnumerator()
	{
		return _teams.GetEnumerator();
	}
}
