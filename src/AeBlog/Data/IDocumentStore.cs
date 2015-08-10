using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface IDocumentStore
    {
        Task<TItem> GetItemAsync<TItem>(string key, string tableName, CancellationToken ctx = default(CancellationToken)) where TItem : class;
    }
}