using LearningBlazor.Utilities.TicTacToe;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;

namespace LearningBlazor.Hubs;
public class TicTacToeHub : Hub
{
	private static readonly List<TicTacToeGame> Games = [];
	private static readonly Dictionary<string, TicTacToePlayer> Players = [];
	private static readonly List<TicTacToePlayer> WaitingPlayers = [];
	private static readonly object gameSearchLock = new(); // ! Mutex for avoiding two or more players finding the same game at the same time

	public override async Task OnConnectedAsync()
	{
		Context.Items["UserInitialized"] = false;
		Context.Items["Game"] = null;
		Context.Items["Opponent"] = null;
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		object userInWaiting = Context.Items["UserInitialized"] ?? false;
		if ((bool)userInWaiting == false)
			return;

		if (Context.Items["Opponent"] is string opponentId)
			await Clients.Client(opponentId).SendAsync("PlayerDisconnected");

		Players.Remove(Context.ConnectionId);
		await base.OnDisconnectedAsync(exception);
	}

	#region Client -> Server Methods
	public async Task SendPlayAgainRequest()
	{
		var game = Context.Items["Game"] as TicTacToeGame ?? throw new Exception("Game is null when sending a play again request!!!");

		if (game.RequestPlayAgain())
		{
            foreach (var p in game.Players)
            {
				await Clients.Client(p.Id).SendAsync("ReceiveResetGame");
            }
        }
	}

	public void HandleOpponentDisconnect()
	{
		var player = Players[Context.ConnectionId];
		WaitingPlayers.Add(player);
		var game = Context.Items["Game"] as TicTacToeGame ?? throw new Exception("Game is null when opponent disconnected!");
		game.RemoveOpponentOf(player);
	}

	public async Task SendMarkData(int i, int j)
    {
		var opponent = Context.Items["Opponent"] as string ?? throw new Exception("Opponent ID is null when sending mark data!!!");
        await Clients.Client(opponent).SendAsync("ReceiveMarkData", i, j);
    }

	public async Task SendUsername(string username)
	{
		await AssignPlayerToMatch(username);
		Context.Items["UserInitialized"] = true;
	}

	public void SetOpponentInContext(string opponentId) => Context.Items["Opponent"] = opponentId;
	#endregion

	#region Server Methods
	private async Task SendSymbolToClient(string symbol)
	{
		await Clients.Caller.SendAsync("ReceiveSymbol", symbol);
	}

	private async Task AssignPlayerToMatch(string username)
	{
		var player = new TicTacToePlayer(Context.ConnectionId, username);
		Players[Context.ConnectionId] = player;

		if (WaitingPlayers.Count == 0)
		{
			await InitializeNewGame(player);

			return;
		}

		TicTacToePlayer? foundOpponent = null;
		bool noValidPlayersInQueue = false;
		if (WaitingPlayers.Count == 0)
		{
			noValidPlayersInQueue = true;
		}
		else
		{
			lock (gameSearchLock) 
			{
				foundOpponent = WaitingPlayers[0];
				WaitingPlayers.RemoveAt(0);
				while (!Players.ContainsKey(foundOpponent!.Id) && WaitingPlayers.Count > 0)
				{
					foundOpponent = WaitingPlayers[0];
					WaitingPlayers.RemoveAt(0);
				}

				noValidPlayersInQueue = !Players.ContainsKey(foundOpponent!.Id) && WaitingPlayers.Count == 0;
			}
		}

		if (noValidPlayersInQueue || foundOpponent is null)
		{
			await InitializeNewGame(player);
			return;
		}

		player.Symbol = "O";
		var game = Games.First(g => g.Players.Contains(foundOpponent));
		game.Players.Add(player);
		Context.Items["Game"] = game;
		Context.Items["Opponent"] = foundOpponent.Id;

		await SendSymbolToClient("O");
		await Clients.Caller.SendAsync("ReceiveStartGame", foundOpponent.Name);
		await Clients.Client(foundOpponent.Id).SendAsync("ReceiveStartGameAndOpponentId", player.Name, player.Id);
	}

	private async Task InitializeNewGame(TicTacToePlayer player)
	{
		player.Symbol = "X";
		await SendSymbolToClient("X");

		WaitingPlayers.Add(player);

		var game = new TicTacToeGame(player);
		Games.Add(game);
		Context.Items["Game"] = game;
	}
	#endregion
}
