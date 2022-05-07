using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ae.Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ae.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public HomeController(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var featuredPosts = (await _blogPostRepository.GetPublishedPosts(token)).Where(x => x.IsFeatured);
            return View(featuredPosts);
        }
    }
}