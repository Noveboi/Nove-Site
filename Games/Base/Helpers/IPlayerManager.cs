using Games.Base.PlayerModel;

namespace Games.Base.Helpers
{
	public interface IPlayerManager
	{
		void ModifyItemInStorage(IPlayer player, Items itemType, object item);
		void RemoveItemFromStorage(IPlayer player, Items itemType);
	}
}