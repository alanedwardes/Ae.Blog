namespace Ae.Blog.Models
{
    public class BlogModel
    {
        public string Category { get; set; }
        public PostSummary[] Archive { get; set; }
        public Post[] Posts { get; set; }
        public Post Single { get; set; }
    }
}
