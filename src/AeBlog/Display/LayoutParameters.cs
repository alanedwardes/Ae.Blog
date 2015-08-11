namespace AeBlog.Display
{
    public class LayoutParameters
    {
        private const string SiteTitle = "Alan Edwardes";

        public LayoutParameters(string title, ViewType viewType)
        {
            this.title = title;
            ViewType = viewType;
        }

        private string title;

        public string Title => string.IsNullOrWhiteSpace(title) ? SiteTitle : $"{title} &mdash; {SiteTitle}";

        public ViewType ViewType { get; }

        public string Theme => ThemePicker.PickRandomTheme();
    }
}
