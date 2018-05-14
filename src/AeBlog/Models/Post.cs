namespace AeBlog.Models
{
    public class Post : PostSummary
    {
        public PostContent Content { get; set; }
        public bool HasMore { get; set; }
        public bool IsSingle { get; set; }
    }
}
