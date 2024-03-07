using LearningBlazor.Utilities.Base;
using LearningBlazor.Utilities.TicTacToe;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace LearningBlazor.Hubs;
public class TicTacToeHub : GameHubBase<TicTacToeGame, TicTacToePlayer>, IGameHub<TicTacToeGame, TicTacToePlayer>
{
	protected static List<TicTacToeGame> Games { get; } = [];
	protected static readonly Dictionary<string, TicTacToePlayer> Players = [];

	// Move these somewhere else (e.g. static class)
	public const string SENDER_CREATE_NEW_GAME = nameof(ExposedCreateNewGame);
	public const string SENDER_CREATE_PLAYER = nameof(ExposedCreatePlayer);
	public const string SENDER_PLAYER_JOIN = nameof(ExposedPlayerJoinGame);
	public const string SENDER_RECEIVE_OPPONENT_ID = nameof(ReceiveOpponentId);
	public const string SENDER_MARK = nameof(MarkBoardAndSend);
	public const string SENDER_OTHER_DISCONNECTED = nameof(ExposedOtherPlayerDisconnected);

	private string OpponentId
	{
		get
		{
			string id = Context.Items[ItemKeys.Opponent] as string ?? string.Empty;
			return id;
		}
		set => Context.Items[ItemKeys.Opponent] = value;
	}

	public override async Task OnConnectedAsync()
	{
		IsUserPlaying = false;

		await SendGameListToClient(Games);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		Players.Remove(Context.ConnectionId);
		await base.OnDisconnectedAsync(exception);
	}

	#region Client -> Server Methods
	public async Task SendPlayAgainRequest()
	{
		if (Game.RequestPlayAgain())
		{
			await Clients.Caller.SendAsync("ReceiveResetGame");
			await Clients.Client(OpponentId).SendAsync("ReceiveResetGame");
		}
	}

	public async Task MarkBoardAndSend(int i, int j) =>
        await Clients.Client(OpponentId).SendAsync(Components.Applets.TicTacToe.RECEIVERS_MARK, i, j);

	public void ReceiveOpponentId(string opponentId) => 
		OpponentId = opponentId;

	public async Task ExposedPlayerJoinGame(string gameNameId)
	{
		await PlayerJoinGame(Games, gameNameId);

		OpponentId = Game.Players[0].Id;

		string symbol = Random.Shared.Next(0, 2) == 1 ? "X" : "O";
		string opponentSymbol = symbol == "X" ? "O" : "X";

		await Clients.Client(OpponentId).SendAsync("ReceiveOpponentId", Context.ConnectionId);

		await Clients.Caller.SendAsync("ReceiveSymbol", symbol);
		await Clients.Client(OpponentId).SendAsync("ReceiveSymbol", opponentSymbol);
		await NotifyGameStart();
	}

	public async Task ExposedCreateNewGame() =>
		await CreateNewGame(Games);

	public async Task ExposedCreatePlayer(string username) =>
		await CreatePlayer(Players, username);

	public async Task ExposedOtherPlayerDisconnected(string connectionId) =>
		await OtherPlayerDisconnected(Players, connectionId);
	#endregion
}
