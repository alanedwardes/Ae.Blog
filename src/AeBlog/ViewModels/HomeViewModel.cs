using AeBlog.Data;
using System.Collections.Generic;

namespace AeBlog.ViewModels
{
    public class HomeViewModel : ArchiveViewModel
    {
        public HomeViewModel(IEnumerable<Post> posts, IEnumerable<Album> albums) : base(posts)
        {
            Albums = albums;
        }

        public IEnumerable<Album> Albums { get; }
    }
}
