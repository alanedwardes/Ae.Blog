using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Clients
{
    public interface ILastfmClient
    {
        Task<IList<JsonAlbum>> GetTopAlbumsForUser(string period, CancellationToken ctx);
    }
}