using Games.Base.Helpers;
using Games.Base.TeamModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Base.GameModel;

/// <summary>
/// Describes a game where winning is achievable.
/// </summary>
internal interface ICanWin
{
	/// <summary>
	/// To be invoked when either win/tie state is true
	/// </summary>
	event EventHandler<GameOverEventArgs> GameOver;
	bool IsWinStateFor(ITeam team);
	bool IsTieState();
}
