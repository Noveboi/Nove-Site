namespace LearningBlazor.Utilities.Base.Player;

[Serializable]
public class PlayerStats : IPlayerStats
{
    public PlayerStats()
    {
        Wins = 0;
        Losses = 0;
        Ties = 0;
    }

    public PlayerStats(int wins, int losses, int ties)
    {
        Wins = wins;
        Losses = losses;
        Ties = ties;
    }

    public int Wins { get; set; } = 0;
    public int Losses { get; set; } = 0;
    public int Ties { get; set; } = 0;

    public double WinPercentage => (double)Wins / (Losses + Ties);
}
