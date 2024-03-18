namespace Games.Base;
public enum GameOverStates
{
	NotOver,
	Win,
	Tie,
	Lose
}
public enum GameStates
{
	Setup,
	Waiting,
	Playing,
	Over
}

/// <summary>
/// "Receivers" are methods that are defined on the client-side.
/// A message specifying which one of the <see cref="Receivers"/> 
/// methods to run is sent from a <see cref="Hub"/> and received by
/// some client that is connected. Thus the message flow is [HUB --> CLIENT].
/// </summary>
public enum Receivers
{
	OnStartGame,
	OnBeginSetup,
	OnFinishSetup,
	OnRestartGame,
	OnGameOver,
	SelfConnected,
	OtherConnected,
	OtherDisconnected,
	GetGameList,
	UpdateGameList,
	GetGameInstance,
}

/// <summary>
/// "Senders" are methods that are defined on the hub-side.
/// A message specifying which one of the <see cref="Senders"/>
/// methods to run is sent from a client and received by the 
/// <see cref="Hub"/> that client is connected to. Thus the message flow is [CLIENT --> HUB].
/// </summary>
public enum Senders
{
	CreateNewGame,
	CreatePlayer,
	ClientJoinGame,
	OtherPlayerDisconnected,
	OnBrowserClose,
	SendGameListToClient,
	ReadyToConnect,
	FinishSetup,
	OtherPlayerConnected
}