using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface IPostManager
    {
        Task<Post> GetPostBySlug(string slug, CancellationToken ctx = default(CancellationToken));
    }
}