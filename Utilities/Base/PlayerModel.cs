namespace LearningBlazor.Utilities.Base;

[Serializable]
public class PlayerModel(string connectionId, string username)
{
    /// <summary>
    /// The connection ID associated with the <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection"/>.
    /// </summary>
    public string Id { get; set; } = connectionId;

    /// <summary>
    /// The 'display name' for the player
    /// </summary>
    public string Name { get; set; } = username;

    /// <summary>
    /// Stores basic game statistics regarding the number of wins, losses, etc...
    /// </summary>
    public GameStatsModel Stats { get; set; } = new();
    public GameOverStates GameOverState { get; set; } = GameOverStates.NotOver;

    public bool IsTurn { get; set; }

    public override string ToString()
        => $"[{Id}] {Name}";
}
