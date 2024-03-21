using Games.Base.TeamModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Base.Helpers;

public class GameOverEventArgs(Dictionary<ITeam, GameOverStates> statesForTeams) : EventArgs
{
	// TODO: Add case where player disconnected in the middle of a game.
	public Dictionary<ITeam, GameOverStates> StatesForTeam { get; } = statesForTeams;
}
