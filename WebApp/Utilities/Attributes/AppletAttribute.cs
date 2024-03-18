using WebApp.Utilities.Models;

namespace WebApp.Utilities.Attributes;

/// <summary>
/// Assign this attribute to Razor Components that function as applets.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
class AppletAttribute(string title, string description, string href) : Attribute
{
    private PageModel _applet = new PageModel(title, description, href);
    public PageModel Applet
    {
        get => _applet;
        protected set => _applet = value;
    }
}
