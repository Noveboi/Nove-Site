using Microsoft.AspNetCore.SignalR;

namespace LearningBlazor.Utilities;
public class GameHubProtocol
{
	public static GameHubProtocol Singleton => new();

	public GameHubProtocol()
	{
		var receiverProtocols = Enum.GetValues<Receivers>();
		var protocolNames = Enum.GetNames<Receivers>();

		for (int i = 0; i < receiverProtocols.Length; i++)
			_receivers[receiverProtocols[i]] = protocolNames[i];

		var senderProtocols = Enum.GetValues<Senders>();
		protocolNames = Enum.GetNames<Senders>();

		for (int i = 0; i < senderProtocols.Length; i++)
			_senders[senderProtocols[i]] = protocolNames[i];

		// Override names for exposed methods in GameHub subclasses
		PrependTextToMethod(Senders.CreateNewGame);
		PrependTextToMethod(Senders.PlayerJoinGame);
		PrependTextToMethod(Senders.CreatePlayer);
		PrependTextToMethod(Senders.OtherPlayerDisconnected);
	}

	private readonly Dictionary<Receivers, string> _receivers = [];
	private readonly Dictionary<Senders, string> _senders = [];

	public string this[Receivers protocol]
	{
		get => _receivers[protocol]; 
	}
	public string this[Senders protocol]
	{
		get => _senders[protocol];
	}

	private void PrependTextToMethod(Senders protocol)
	{
		_senders[protocol] = "Exposed" + _senders[protocol];
	} 
}

/// <summary>
/// "Receivers" are methods that are defined on the client-side.
/// A message specifying which one of the <see cref="Receivers"/> 
/// methods to run is sent from a <see cref="Hub"/> and received by
/// some client that is connected. Thus the message flow is [HUB --> CLIENT].
/// </summary>
public enum Receivers
{
	OnBeginGame,
	SelfConnected,
	OtherConnected,
	OtherDisconnected,
	GetGameList,
	GetPlayerModel,
	UpdateGameList
}

/// <summary>
/// "Senders" are methods that are defined on the hub-side.
/// A message specifying which one of the <see cref="Senders"/>
/// methods to run is sent from a client and received by the 
/// <see cref="Hub"/> that client is connected to. Thus the message flow
/// is [CLIENT --> HUB].
/// </summary>
public enum Senders
{
	CreateNewGame,
	CreatePlayer,
	PlayerJoinGame,
	OtherPlayerDisconnected,
	OnBrowserClose
}