using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface ICategoryManager
    {
        Task<IEnumerable<Category>> GetPublishedCategories(CancellationToken ctx = default(CancellationToken));
    }
}
