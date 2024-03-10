using LearningBlazor.Utilities.Models;

namespace LearningBlazor.Utilities.Attributes;

/// <summary>
/// Assign this attribute to Razor Components that function as applets.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class AppletAttribute(string title, string description, string href) : Attribute
{
    public PageModel Applet => new(title, description, href);
}
