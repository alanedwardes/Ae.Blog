using AeBlog.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Services
{
    public interface IBlogPostRetriever
    {
        Task<PostSummary[]> GetPublishedPostSummaries(CancellationToken token);
        Task<Post[]> GetPostsForCategory(string category, CancellationToken token);
        Task<Post[]> GetPublishedPosts(CancellationToken token);
        Task<PostSummary[]> GetAllPostSummaries(CancellationToken token);
        Task<Post> GetPost(string slug, CancellationToken token);
        Task PutPost(Post post, CancellationToken token);
    }
}