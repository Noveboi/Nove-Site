using Games.Base.Helpers;

namespace Games.Base.PlayerModel;
public interface IPlayer
{
	public string ConnectionId { get; set; }
	public string Name { get; set; }

	/// <summary>
	/// Contains any extra game-specific information and data
	/// </summary>
	public IDictionary<Items, object> Storage { get; }

	/// <summary>
	/// Contains basic statistics for every game type.
	/// </summary>
	public IDictionary<string, IGameStatistics> GameStatistics { get; }
}