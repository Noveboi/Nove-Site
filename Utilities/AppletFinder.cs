using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace LearningBlazor.Utilities
{
	public static class AppletFinder
	{
		public static IEnumerable<AppletModel> GetApplets()
		{
			var razorComponents = Assembly
				.GetExecutingAssembly()
				.ExportedTypes
				.Where(t => t.IsSubclassOf(typeof(ComponentBase)));

			IEnumerable<AppletModel> appletList = [];

			foreach (var component in razorComponents)
			{
				AppletAttribute? appletAttribute;
				if ((appletAttribute = component.GetCustomAttribute<AppletAttribute>(inherit: false)) is not null)
				{
					yield return appletAttribute.Applet;	
				}
			}
		}
	}
}
