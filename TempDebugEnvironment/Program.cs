using Games.Base.GameModel;
using Games.Base;
using Games.Base.PlayerModel;
using Games.Base.TeamModel;
using Games.TicTacToe;

// Demo 
// 1. Create players
IPlayer player1 = PlayerFactory.CreatePlayer(Guid.NewGuid().ToString(), "Nove");
IPlayer player2 = PlayerFactory.CreatePlayer(Guid.NewGuid().ToString(), "Bobby");

// 2. Create game
var game = GameFactory.CreateGame<TicTacToeGame>("Nove's Game");

Console.WriteLine($"Created game {game}");

// 3. Create teams
var team1 = TeamFactory.CreateTeam("Nove's team", [player1]);
var team2 = TeamFactory.CreateTeam("Ricky's team", [player2]);

game.Teams.Add(team1);
game.Teams.Add(team2);

game.PlayerManager.ModifyItemInStorage(player1, Items.Symbol, "X");
game.PlayerManager.ModifyItemInStorage(player2, Items.Symbol, "O");

game.Start();

Console.WriteLine("Game started.");
Console.WriteLine($"{player1.Name}'s Symbol: {player1.Storage[Items.Symbol]}");
Console.WriteLine($"{player2.Name}'s Symbol: {player2.Storage[Items.Symbol]}");