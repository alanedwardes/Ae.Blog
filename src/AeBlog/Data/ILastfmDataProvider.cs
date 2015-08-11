using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface ILastfmDataProvider
    {
        Task<IEnumerable<Album>> GetTopAlbumsForUser(string user, string api_key, string period, CancellationToken ctx = default(CancellationToken));
    }
}