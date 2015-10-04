using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks.Helpers
{
    public interface IDynamoTableRetriever
    {
        Task RetrieveTable<TItem>(string tableName, CancellationToken ctx);
    }
}
