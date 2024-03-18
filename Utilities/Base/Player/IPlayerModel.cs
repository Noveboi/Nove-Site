namespace LearningBlazor.Utilities.Base.Player;
public interface IPlayerModel
{
	public string ConnectionId { get; set; }
	public string Name { get; set; }
	public PlayerStats Stats { get; set; }
	public GameOverStates GameOverState { get; set; }
	public bool HasTurn { get; set; }
}
