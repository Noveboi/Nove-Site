namespace LearningBlazor.Utilities.Base.Player;

[Serializable]
public class PlayerModel(string connectionId, string username, PlayerStats stats) : IPlayerModel
{
    /// <summary>
    /// The connection ID associated with the <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection"/>.
    /// </summary>
    public string ConnectionId { get; set; } = connectionId;

    /// <summary>
    /// The 'display name' for the player
    /// </summary>
    public string Name { get; set; } = username;

    /// <summary>
    /// Stores basic game statistics regarding the number of wins, losses, etc...
    /// </summary>
    public PlayerStats Stats { get; set; } = stats;
    public GameOverStates GameOverState { get; set; } = GameOverStates.NotOver;

    public bool HasTurn { get; set; }

    public override string ToString()
        => $"[{ConnectionId}] {Name}";
}
