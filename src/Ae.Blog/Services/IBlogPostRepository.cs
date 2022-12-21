using Ae.Blog.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Ae.Blog.Services
{
    public interface IBlogPostRepository
    {
        Task<Post[]> GetPublishedPosts(CancellationToken token);
        Task<PostSummary[]> GetAllContentSummaries(CancellationToken token);
        Task<Post> GetContent(string slug, CancellationToken token);
        Task PutContent(Post post, CancellationToken token);
        Task DeleteContent(string slug, CancellationToken token);
    }
}