using Games.Base.PlayerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Base.TeamModel;

public static class TeamFactory
{
	public static ITeam CreateTeam()
    {
        return new Team("Team", new List<IPlayer>());
    }

	public static ITeam CreateTeam(string name)
    {
        return new Team(name, new List<IPlayer>());   
    }

	public static ITeam CreateTeam(string name, List<IPlayer> players)
    {
        return new Team(name, players);
    }
}
