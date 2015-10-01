using System.Collections.Generic;
using System.Threading.Tasks;

namespace AeBlog
{
    public interface ICacheProvider
    {
        Task<TObject> Get<TObject>(string key);
        Task Set<TObject>(string key, TObject value);
    }
}
