using AeBlog.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Services
{
    public interface IBlogPostRetriever
    {
        Task<IEnumerable<PostSummary>> GetPosts(CancellationToken token);
        Task<PostSummary> GetPost(string slug, CancellationToken token);
    }
}