using System.Collections.Generic;
using System.Threading.Tasks;

namespace AeBlog.Caching
{
    public class MemoryCache : ICacheProvider
    {
        public static ICacheProvider INSTANCE { get; } = new MemoryCache();

        private MemoryCache()
        {
        }

        private IDictionary<string, object> cache = new Dictionary<string, object>();

        public Task<TObject> Get<TObject>(string key)
        {
            if (!cache.ContainsKey(key))
            {
                return Task.FromResult(default(TObject));
            }

            return Task.FromResult((TObject)cache[key]);
        }

        public Task Set<TObject>(string key, TObject value)
        {
            lock (cache)
            {
                if (cache.ContainsKey(key))
                {
                    cache[key] = value;
                }
                else
                {
                    cache.Add(key, value);
                }
            }

            return Task.CompletedTask;
        }
    }
}
