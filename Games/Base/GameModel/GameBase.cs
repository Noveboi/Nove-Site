using Games.Base.Helpers;
using Games.Base.PlayerModel;
using Games.Base.Team;
using Games.Base.TeamModel;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Games.Base.GameModel;

[Serializable]
public abstract class GameBase 
{
    /// <summary>
    /// Identifies an instance of a game of any type.
    /// </summary>
    public abstract UniqueGameId UniqueId { get; }

    /// <summary>
    /// DO NOT INSTANTIATE OUTSIDE OF <see cref="Games"/> ASSEMBLY!
    /// </summary>
	internal GameBase(IEnumerable<ITeam> teams)
    {
        Title = GetHashCode().ToString();
        Teams = new(teams);
        PlayerManager = new PlayerManager();
    }

	/// <summary>
	/// DO NOT INSTANTIATE OUTSIDE OF <see cref="Games"/> ASSEMBLY!
	/// </summary>
	internal GameBase(string name, IEnumerable<ITeam> teams)
    {
        Title = name;
        Teams = new(teams);
        PlayerManager = new PlayerManager();
    }

	/// <summary>
	/// Display name that is shown to the end-user.
	/// </summary>
	public string Title { get; set; }

    public GameStates State { get; set; } = GameStates.Waiting;
    public IPlayerManager PlayerManager { get; set; }
    public TeamCollection Teams { get; set; }

    [JsonIgnore] // because it is calculated from Teams, no need to include in serialization
    public int PlayerCount
    {
        get
        {
            int count = 0;
            foreach (ITeam team in Teams)
            {
                count += team.Count;
            }
            return count;
        }
    }
    public abstract int PlayerCapacity { get; }

    /// <summary>
    /// Called when <see cref="GameStates.Setup"/> is desired. Here, players do any customization/setup they want.
    /// The customization is then reflected in the <see cref="IPlayer.Storage"/> dictionary.
    /// </summary>
    public abstract void Setup();
    /// <summary>
    /// Called when <see cref="GameStates.Setup"/> has completed and the transition to <see cref="GameStates.Playing"/> 
    /// is ready to happen.
    /// </summary>
    public abstract void Start();
    /// <summary>
    /// Resets all game variables to their initial state and sets the <see cref="GameStates"/> according to
    /// the <paramref name="toSetup"/> parameter.
    /// </summary>
    /// <param name="toSetup">
    ///     By default, restart and set the state to <see cref="GameStates.Playing"/>.
    ///     If true, restart and set the state to <see cref="GameStates.Setup"/>.
    /// </param>
    public abstract void Restart(bool toSetup = false);
    public abstract void HandleEndByDisconnect();
    public abstract void HandleEndByWinOrTie();
    /// <summary>
    /// Called when the game is over, either due to a normal "Game Over" or due to some player disconnecting.
    /// </summary>
    /// <param name="gameEndedBy"></param>
    public void End(GameEndedBy gameEndedBy)
    {
		switch (gameEndedBy)
		{
			case GameEndedBy.PlayerDisconnect:
                HandleEndByDisconnect();
				break;
			case GameEndedBy.WinOrTie:
                HandleEndByWinOrTie();
				break;
		}
	}

	public override string ToString() =>
        $"\"{Title}\" ({PlayerCount} / {PlayerCapacity} Players)";
}
