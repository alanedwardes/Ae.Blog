using AeBlog.Data;

namespace AeBlog.ViewModels
{
    public class SinglePostViewModel
    {
        public SinglePostViewModel(Post post)
        {
            Post = post;
        }

        public Post Post { get; }
    }
}
