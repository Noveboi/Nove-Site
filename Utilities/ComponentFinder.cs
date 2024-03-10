using LearningBlazor.Components.Pages;
using LearningBlazor.Utilities.Attributes;
using LearningBlazor.Utilities.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace LearningBlazor.Utilities;

/// <summary>
/// Uses reflection to search for Razor Components.
/// </summary>
public static class ComponentFinder
{
	/// <summary>
	/// Searches the executing <see cref="Assembly"/> for public subclasses of the type <see cref="ComponentBase"/> that are
	/// tagged the with <see cref="AppletAttribute"/> class 
	/// </summary>
	/// <returns> An enumerable collection of <see cref="PageModel"/> types</returns>
	public static IEnumerable<PageModel> GetApplets()
	{
		foreach (var component in GetRazorComponents())
			if (component.GetCustomAttribute<AppletAttribute>(inherit: false) is AppletAttribute appletAttr)
				yield return appletAttr.Applet;	
	}

	/// <summary>
	/// Searches the executing <see cref="Assembly"/> for public subclasses of the type <see cref="ComponentBase"/>.
	/// The search EXCLUDES pages that are tagged with applet attributes and base pages like "Home" and "Error". 
	/// The ignored pages are listed in the property <see cref="_pageIgnore"/>
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<PageModel> GetMainPages()
	{
		foreach (var component in GetRazorComponents())
			if (component.GetCustomAttribute<MainPageAttribute>(inherit: false) is MainPageAttribute mainPageAttr)
				yield return mainPageAttr.Page;
	}

	private static IEnumerable<Type> _pageIgnore =>
		[typeof(Home), typeof(Error)];

	private static IEnumerable<Type> GetRazorComponents() => 
		Assembly
			.GetExecutingAssembly()
			.ExportedTypes
			.Where(t => t.IsSubclassOf(typeof(ComponentBase)));
}
