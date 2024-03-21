using System.Text;
using System.Reflection.Metadata;

namespace Games.Base.GameModel;

public class UniqueGameId(Type gameType)
{
	private readonly Guid _uniqueId = Guid.NewGuid();
	private readonly string _gameType = gameType.Name;

	public override string ToString()
		=> $"{_uniqueId}_{_gameType}";
}
