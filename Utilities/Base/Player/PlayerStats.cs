namespace LearningBlazor.Utilities.Base.Player;

[Serializable]
public class PlayerStats
{
    public int Wins { get; set; } = 0;
    public int Losses { get; set; } = 0;
    public int Ties { get; set; } = 0;

    public double WinPercent => (double)Wins / (Losses + Ties);
}
