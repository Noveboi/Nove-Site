namespace LearningBlazor.Utilities
{
	public interface IPlayer
	{
		/// <summary>
		/// The connection ID of the player's client. Required for SignalR Hubs
		/// </summary>
		string Id { get; }

		/// <summary>
		/// The player's username
		/// </summary>
		string Name { get; }
	}
}
