namespace LearningBlazor.Utilities.Base.Hubs;
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
        PrependTextToMethod(Senders.ClientJoinGame);
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

