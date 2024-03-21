namespace Games.Base.Helpers;

public interface IGameStatistics
{
	public int Wins { get; set; }
	public int Losses { get; set; }
	public int Ties { get; set; }
}
