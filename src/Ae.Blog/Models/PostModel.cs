namespace Ae.Blog.Models
{
    public class PostModel
    {
        public PostModel(Post post, bool isSingle)
        {
            Post = post;
            IsSingle = isSingle;
        }

        public Post Post { get; }
        public bool IsSingle { get; }
    }
}
