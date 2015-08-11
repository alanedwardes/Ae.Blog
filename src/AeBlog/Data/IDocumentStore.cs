using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface IDocumentStore
    {
        Task<TItem> GetItem<TItem>(string key, string tableName, CancellationToken ctx = default(CancellationToken)) where TItem : class;
        Task<IEnumerable<TItem>> GetItems<TItem>(string tableName, CancellationToken ctx = default(CancellationToken)) where TItem : class;
    }
}