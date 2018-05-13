using System.Threading;
using System.Threading.Tasks;
using AeBlog.Models;
using AeBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AeBlog.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> logger;
        private readonly IBlogPostRetriever blogPostRetriever;

        public BlogController(ILogger<BlogController> logger, IBlogPostRetriever blogPostRetriever)
        {
            this.logger = logger;
            this.blogPostRetriever = blogPostRetriever;
        }

        public async Task<IActionResult> Index()
        {
            return View("List", new BlogModel
            {
                Archive = await blogPostRetriever.GetPostSummaries(CancellationToken.None),
                Posts = await blogPostRetriever.GetPosts(CancellationToken.None)
            });
        }

        public async Task<IActionResult> Posts(string id)
        {
            return View("Single", new BlogModel
            {
                Archive = await blogPostRetriever.GetPostSummaries(CancellationToken.None),
                Single = await blogPostRetriever.GetPost(id, CancellationToken.None)
            });
        }

        public async Task<IActionResult> Category(string id)
        {
            return View("List", new BlogModel
            {
                Archive = await blogPostRetriever.GetPostSummaries(CancellationToken.None),
                Posts = await blogPostRetriever.GetPostsForCategory(id, CancellationToken.None)
            });
        }
    }
}