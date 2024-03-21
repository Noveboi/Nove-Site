using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Base.Helpers;

internal class GameStatistics : IGameStatistics
{
	public int Wins { get; set; } = 0;
	public int Losses { get; set; } = 0;
	public int Ties { get; set; } = 0;
}