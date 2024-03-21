using Games.Base.Team;
using Games.Base.TeamModel;

namespace Games.Base.Helpers;

public interface ITurnManager
{
	ITeam PlayingTeam { get; }
	void NextTurn();
	void Activate(TeamCollection teams);
}
