namespace LearningBlazor.Utilities
{
    /// <summary>
    /// Assign this attribute to Razor pages that functions as applets.
    /// The home page will then automatically display a card with information 
    /// taken from the <see cref="AppletModel"/> class
    /// </summary>
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class AppletAttribute(string title, string description, string href) : Attribute
    {
        readonly AppletModel applet = new AppletModel(title, description, href);

        public AppletModel Applet => applet;
    }
}
