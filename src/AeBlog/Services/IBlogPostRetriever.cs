using AeBlog.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Services
{
    public interface IBlogPostRetriever
    {
        Task<PostSummary[]> GetPostSummaries(CancellationToken token);
        Task<Post[]> GetPostsForCategory(string category, CancellationToken token);
        Task<Post[]> GetPosts(CancellationToken token);
        Task<Post> GetPost(string slug, CancellationToken token);
    }
}