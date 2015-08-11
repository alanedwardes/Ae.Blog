namespace AeBlog.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorViewModel(string title, string description = null)
        {
            Title = title;
            Description = description;
        }

        public string Description { get; }
        public string Title { get; }
    }
}
