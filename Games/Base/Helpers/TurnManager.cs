using Games.Base.Team;
using Games.Base.TeamModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Base.Helpers;

public class TurnManager : ITurnManager
{
	private TeamCollection _teams = null!;
	private int _index = 0;
	private bool _readyToUse = false;

	public ITeam PlayingTeam
	{
		get => _readyToUse ? _teams[_index] : throw new Exception("TurnManager is not activated!");
	}

	public void Activate(TeamCollection teams)
	{
		_readyToUse = true;
		_teams = teams;
	}

	public void NextTurn()
	{
		if (_readyToUse)
		{
			_index = (_index + 1) % _teams.Count;
		}
		else
		{
			throw new Exception("TurnManager is not activated!");
		}
	}
}
