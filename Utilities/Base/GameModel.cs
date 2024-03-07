namespace LearningBlazor.Utilities.Base;

[Serializable]
public class GameModel
{
    public GameModel()
    {
        NameId = GetHashCode().ToString();
        Players = [];
    }

    public GameModel(PlayerModel player1)
    {
        NameId = GetHashCode().ToString();
        Players = [player1];
    }

    public GameModel(PlayerModel player1, PlayerModel player2)
    {
        NameId = GetHashCode().ToString();
        Players = [player1, player2];
    }

    public GameModel(string name, PlayerModel player1)
    {
        NameId = name;
        Players = [player1];
    }

    public GameModel(string name, PlayerModel player1, PlayerModel player2)
    {
        NameId = name;
        Players = [player1, player2];
    }

    public virtual int PlayerCapacity { get; set; }
    public string NameId { get; set; }

    public List<PlayerModel> Players { get; set; }

    public override string ToString()
        => $"\"{NameId}\" ({Players.Count} / {PlayerCapacity} Players)";
}
