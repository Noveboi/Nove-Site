using Games.Base.Helpers;
using Games.Base.TeamModel;

namespace Games.Base.GameModel;

/// <summary>
/// Describes games that are turn-based 
/// </summary>
public interface ITurnBased
{
	ITurnManager TurnManager { get; }
}
