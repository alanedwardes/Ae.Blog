using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Caching
{
    public interface ICacheProvider
    {
        Task<TObject> Get<TObject>(string key, CancellationToken ctx);
        Task Set<TObject>(string key, TObject value, CancellationToken ctx);
    }
}
