using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface IDocumentStore
    {
        Task<IEnumerable<TItem>> GetItems<TItem>(string tableName, string key, object value, string index, CancellationToken ctx) where TItem : class;
        Task<IEnumerable<TItem>> GetItems<TItem>(string tableName, CancellationToken ctx) where TItem : class;
        Task StoreBinaryItem<TItem>(string tableName, TItem item, string key, byte[] bytes) where TItem : class;
        Task<TItem> GetBinaryItem<TItem>(string tableName, string key, object value, string binaryKey, Stream stream) where TItem : class;
    }
}