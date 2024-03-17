namespace LearningBlazor.Utilities.Base;

[Serializable]
public class GameModel<TPlayer>
{
	#region Constructors
	public GameModel()
    {
        NameId = GetHashCode().ToString();
        Players = [];
    }

    public GameModel(string name)
    {
        NameId = name;
        Players = [];
    }

    public GameModel(IEnumerable<TPlayer> playerCollection)
    {
        NameId = GetHashCode().ToString();
        Players = playerCollection.ToList();
    }

    public GameModel(string name, IEnumerable<TPlayer> playerCollection)
    {
        NameId = name;
        Players = playerCollection.ToList();
    }
	#endregion
    /// <summary>
    /// Unique identifier for each instance.
    /// </summary>
	public string NameId { get; set; }

    public GameStates State { get; set; } = GameStates.Waiting;

    public List<TPlayer> Players { get; set; }
	public virtual int PlayerCapacity { get; set; }

    public virtual void Restart() => 
        State = GameStates.Playing;

	public override string ToString() => 
        $"\"{NameId}\" ({Players.Count} / {PlayerCapacity} Players)";
}
