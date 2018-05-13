namespace AeBlog.Models
{
    public class BlogModel
    {
        public PostSummary[] Archive { get; set; }
        public Post[] Posts { get; set; }
        public Post Single { get; set; }
    }
}
