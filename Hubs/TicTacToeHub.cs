using LearningBlazor.Utilities.Base;
using LearningBlazor.Utilities.TicTacToe;
using Microsoft.AspNetCore.SignalR;

namespace LearningBlazor.Hubs;
public class TicTacToeHub : GameHubBase<TicTacToeGame, TicTacToePlayer>, IGameHub<TicTacToeGame, TicTacToePlayer>
{
	private static readonly List<TicTacToeGame> _games = [];
	private static readonly Dictionary<string, TicTacToePlayer> _players = [];

	// Move these somewhere else (e.g. static class)
	public const string SENDER_RECEIVE_OPPONENT_ID = nameof(ReceiveOpponentId);
	public const string SENDER_MARK = nameof(MarkBoardAndSend);

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

		await SendGameListToClient(_games);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_players.Remove(Context.ConnectionId);
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
		await PlayerJoinGame(_games, gameNameId);

		OpponentId = Game.Players[0].Id;

		string symbol = Random.Shared.Next(0, 2) == 1 ? "X" : "O";
		string opponentSymbol = symbol == "X" ? "O" : "X";

		await Clients.Client(OpponentId).SendAsync("ReceiveOpponentId", Context.ConnectionId);

		await Clients.Caller.SendAsync("ReceiveSymbol", symbol);
		await Clients.Client(OpponentId).SendAsync("ReceiveSymbol", opponentSymbol);
		await NotifyGameStart();
	}

	public async Task ExposedCreateNewGame() =>
		await CreateNewGame(_games);

	public async Task ExposedCreatePlayer(string username) =>
		await CreatePlayer(_players, username);

	public async Task ExposedOtherPlayerDisconnected(string connectionId) =>
		await OtherPlayerDisconnected(_players, connectionId);
	#endregion
}
