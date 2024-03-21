using Games.Base;
using WebApp.Hubs;
using WebApp.Utilities.Services;
using Games.TicTacToe;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Serilog;

namespace Games.Hubs;
public class TicTacToeHub : GameHub<TicTacToeGame, TicTacToePlayer>, IExposeMethods<TicTacToeGame, TicTacToePlayer>
{
	private static readonly List<TicTacToeGame> _games = [];
	private static readonly Dictionary<string, TicTacToePlayer> _players = [];
	private static int _playAgainVotes = 0;
	private static bool _xGoesFirst = Random.Shared.Next(0, 2) == 0;

	private string OpponentId
	{
		get => Context.Items[ItemKeys.Opponent] as string ?? string.Empty;
		set => Context.Items[ItemKeys.Opponent] = value;
	}

	public override async Task OnConnectedAsync()
	{
		await SendGameListToClient(_games);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_players.Remove(Context.ConnectionId);
		await base.OnDisconnectedAsync(exception);
	}

	#region Client -> Server Methods
	public async Task VoteToPlayAgain()
	{
		_playAgainVotes++;
		if (_playAgainVotes >= Game.Players.Count)
		{
			_playAgainVotes = 0;
			_xGoesFirst = Random.Shared.Next(0, 2) == 0;
			await RestartGame();
		}
	}

	public async Task GetMarkDataFromClient(int i, int j)
	{
		Log.Information("{Player} received mark data for [{x}, {y}]", Player.Name, i, j);

		// Update game object board
		Game.Board.Mark(Player.Symbol, i, j);

		// Swap player turns
		Game.NextTurn();


		// TODO: Update Player Stats
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// TODO 2: MAKE BASE GAME HAVE IsWinStateFor and IsTieState Method!!!!!!
		// Check for game over conditions
		if (Game.IsWinStateFor(Player))
		{
			Log.Information("Detected {Win} state for {Player}", "Win", Player.Name);
			var selfPlayer = Player;
			var opponentPlayer = _players[OpponentId];

			// Update player objects
			selfPlayer.GameOverState = GameOverStates.Win;
			opponentPlayer.GameOverState = GameOverStates.Lose;

			Game.State = GameStates.Over;
		}
		if (Game.IsTieState())
		{
			var selfPlayer = Player;
			var opponentPlayer = _players[OpponentId];

			selfPlayer.GameOverState = GameOverStates.Tie;
			opponentPlayer.GameOverState = GameOverStates.Tie;

			Game.State = GameStates.Over;
		}

		// Send game data back to players
		await SendGameToPlayers();
	}


	public override async Task ReadyToConnect()
	{
		await base.ReadyToConnect();
		
		if (Game.Players.Count == 2)
		{
			OpponentId = Game.Players[0].ConnectionId;

			await BeginSetup();
		}
	}

	public override async Task FinishSetup(string arg)
	{
		string symbol = JsonConvert.DeserializeObject<string>(arg) ?? "X";
		string opponentSymbol = symbol == "X" ? "O" : "X";
		Player.Symbol = symbol;
		_players[OpponentId].Symbol = opponentSymbol;

		Log.Information("{Player} is assigned the symbol: {Symbol}", Player.Name, symbol);
		Log.Information("{Player} is assigned the symbol: {Symbol}", _players[OpponentId].Name, opponentSymbol);

		if (Player.Symbol == "X" && _xGoesFirst || Player.Symbol == "O" && _xGoesFirst == false)
			Player.HasTurn = true;
		else
			_players[OpponentId].HasTurn = true;

		await base.FinishSetup(arg);
	}

	public override Task OtherPlayerConnected()
	{
		OpponentId = Game.Players[1].ConnectionId;
		return base.OtherPlayerConnected();
	}

	public async Task ExposedClientJoinGame(string gameNameId) =>
		await ClientJoinGame(_games, gameNameId);

	public async Task ExposedCreateNewGame() =>
		await CreateNewGame(_games);

	public async Task ExposedCreatePlayer(string username) =>
		await CreatePlayer(_players, username);

	public async Task ExposedOtherPlayerDisconnected(string connectionId) =>
		await OtherPlayerDisconnected(_players, connectionId);
	#endregion
}
