using WebApp.Utilities.Models;

namespace WebApp.Utilities.Attributes;

/// <summary>
/// Use this attribute on pages that are directly navigable from the "Home" page
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class MainPageAttribute : Attribute
{
	public PageModel Page { get; }

	public MainPageAttribute(string title, string href)
	{
		Page = new PageModel(title, string.Empty, href);
	}

	public MainPageAttribute(string title, string description, string href)
	{
		Page = new PageModel(title, description, href);
	}
}
