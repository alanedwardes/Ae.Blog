using AeBlog.Clients;
using AeBlog.Data;
using System.Collections.Generic;

namespace AeBlog.ViewModels
{
    public class HomeViewModel : ArchiveViewModel
    {
        public HomeViewModel(IEnumerable<Post> posts, IEnumerable<Portfolio> portfolios, IEnumerable<Album> albums) : base(posts)
        {
            Portfolios = portfolios;
            Albums = albums;
        }

        public IEnumerable<Portfolio> Portfolios { get; }
        public IEnumerable<Album> Albums { get; }
    }
}
