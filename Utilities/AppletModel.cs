namespace LearningBlazor.Utilities
{
	public class AppletModel(string title, string description, string href)
    {
        public string Title { get; } = title;
        public string Description { get; } = description;
        public string Href { get; } = href;
    }
}
