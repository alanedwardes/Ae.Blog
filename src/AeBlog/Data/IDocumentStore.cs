using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface IDocumentStore
    {
        Task<IEnumerable<TItem>> GetItems<TItem>(string tableName, string key, object value, string index, CancellationToken ctx = default(CancellationToken)) where TItem : class;
        Task<IEnumerable<TItem>> GetItems<TItem>(string tableName, CancellationToken ctx = default(CancellationToken)) where TItem : class;
    }
}