using AeBlog.Data;
using System.Collections.Generic;

namespace AeBlog.ViewModels
{
    public class ArchiveViewModel
    {
        public ArchiveViewModel(IEnumerable<Post> posts)
        {
            Posts = posts;
        }

        public IEnumerable<Post> Posts { get; }
    }
}
