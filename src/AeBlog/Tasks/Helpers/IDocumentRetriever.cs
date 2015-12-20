using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks.Helpers
{
    public interface IDocumentRetriever
    {
        Task RetrieveDocuments<TItem>(string tableName, CancellationToken ctx);
    }
}
