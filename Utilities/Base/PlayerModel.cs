namespace LearningBlazor.Utilities.Base;

[Serializable]
public class PlayerModel(string connectionId, string username)
{
    public string Id { get; set; } = connectionId;
    public string Name { get; set; } = username;
}
