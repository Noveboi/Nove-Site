using Games.Base.Helpers;
using Newtonsoft.Json;

namespace Games.Base.PlayerModel;

[Serializable]
public class Player : IPlayer
{
    // An 'assembly-only' constructor used for PlayerFactory. Do NOT expose this constructor to other assemblies
    internal Player(string connId, string username, IDictionary<Items, object> storage, IDictionary<string, IGameStatistics> gameStats)
    {
        ConnectionId = connId;
        Name = username;
        Storage = storage;
		GameStatistics = gameStats;
    }

    /// <summary>
    /// The connection ID associated with the <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection"/>.
    /// </summary>
    public string ConnectionId { get; set; } 

    /// <summary>
    /// The 'display name' for the player
    /// </summary>
    public string Name { get; set; } 

	public IDictionary<Items, object> Storage { get; }

	public IDictionary<string, IGameStatistics> GameStatistics { get; }

	public override string ToString()
        => $"[{ConnectionId}] {Name}";
}
