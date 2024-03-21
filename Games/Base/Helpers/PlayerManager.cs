using Games.Base.PlayerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Base.Helpers;

public class PlayerManager : IPlayerManager
{
	public void ModifyItemInStorage(IPlayer player, Items itemType, object item)
	{
		player.Storage[itemType] = item;
	}

	public void RemoveItemFromStorage(IPlayer player, Items itemType)
	{
		player.Storage.Remove(itemType);
	}

}
