using AeBlog.Data;
using System.Collections.Generic;

namespace AeBlog.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel(IEnumerable<Post> posts)
        {
            Posts = posts;
        }

        public IEnumerable<Post> Posts { get; }
    }
}
