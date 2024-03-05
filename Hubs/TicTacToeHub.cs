using LearningBlazor.Components.Applets;
using LearningBlazor.Utilities.Base;
using LearningBlazor.Utilities.TicTacToe;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace LearningBlazor.Hubs;
public class TicTacToeHub : GameHub<TicTacToeGame, TicTacToePlayer>, IGameHub
{
	protected static readonly List<TicTacToeGame> Games = [];
	protected static readonly Dictionary<string, TicTacToePlayer> Players = [];

	public const string SENDER_CREATE_NEW_GAME = nameof(CreateNewGame);
	public const string SENDER_CREATE_PLAYER = nameof(CreatePlayer);
	public const string SENDER_PLAYER_JOIN = nameof(ClientJoinGame);
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

		await SendGameListToClient(Games);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		if (IsUserPlaying == false)
			return;

		if (OpponentId != string.Empty)
			await Clients.Client(OpponentId).SendAsync(GameComponent.RECEIVERS_PLAYER_DISCONNECTED);

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

	public Task HandleOtherPlayerDisconnect()
	{
		var player = Players[Context.ConnectionId];
		Game.RemoveOpponentOf(player);

		return Task.CompletedTask;
	}

	public async Task MarkBoardAndSend(int i, int j) =>
        await Clients.Client(OpponentId).SendAsync(TicTacToe.RECEIVERS_MARK, i, j);

	public void ReceiveOpponentId(string opponentId) => 
		OpponentId = opponentId;

	public Task CreatePlayer(string username)
	{
		Players[Context.ConnectionId] = new TicTacToePlayer(Context.ConnectionId, username);
		Player = Players[Context.ConnectionId];
		return Task.CompletedTask;
	}
	public async Task ClientJoinGame(string gameNameId)
	{
		var game = Games.FirstOrDefault(g => g.NameId == gameNameId)
			?? throw new Exception("Game not found!");

		Game = game;

		await NotifyPlayersOfConnect();
		game.Players.Add(Player);
		OpponentId = game.Players[0].Id;

		string symbol = Random.Shared.Next(0, 2) == 1 ? "X" : "O";
		string opponentSymbol = symbol == "X" ? "O" : "X";

		await Clients.Client(OpponentId).SendAsync("ReceiveOpponentId", Context.ConnectionId);
		await Clients.Caller.SendAsync("ReceiveSymbol", symbol);
		await Clients.Client(OpponentId).SendAsync("ReceiveSymbol", opponentSymbol);
		await NotifyGameStart();
	}

	public async Task CreateNewGame()
	{
		var game = new TicTacToeGame(Player);
		Games.Add(game);
		Game = game;

		// For anyone else in this hub, send the updated game list 
		await Clients.Others.SendAsync(GameComponent.RECEIVERS_UPDATE_GAME_LIST, JsonConvert.SerializeObject(game));
	} 
	#endregion
}
