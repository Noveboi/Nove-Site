namespace Games.Base.Player;

/// <summary>
/// Base interface for storing information on player stats and performance
/// </summary>
public interface IPlayerStats
{
	int Wins { get; set; }
	int Losses { get; set; }
	int Ties { get; set; }
	double WinPercentage { get; }
}
