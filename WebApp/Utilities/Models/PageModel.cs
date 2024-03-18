namespace WebApp.Utilities.Models;

/// <summary>
/// General purpose model that contains basic information about a Razor Component 
/// </summary>
/// <param name="title">The component's title</param>
/// <param name="description">A brief description of the component</param>
/// <param name="href">The route endpoint of the component</param>
public class PageModel(string title, string description, string href)
{
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string Href { get; } = href;
}
