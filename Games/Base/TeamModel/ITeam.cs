using Games.Base.PlayerModel;
using Microsoft.AspNetCore.Mvc;

namespace Games.Base.TeamModel;

public interface ITeam : IEnumerable<IPlayer> 
{
	string Name { get; set; }

	IPlayer this[int index] { get; }
	IPlayer? this[string connectionId] { get; }
	int Count { get; }
	void AddPlayer(IPlayer player);
	void RemovePlayer(IPlayer player);
	void OnGameOver(GameOverStates gameOverState);
}