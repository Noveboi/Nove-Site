namespace LearningBlazor.Utilities.Attributes;
sealed class GameAttribute : AppletAttribute
{
	internal GameAttribute(string title, string description, Type gameType) : base(title, description, "applets")
	{
		var href = Applet.Href;
		Applet = new Models.PageModel(title, description, $"{href}/{gameType.Name}/lobby");
	} 
}
