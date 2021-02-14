using System;
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
        private readonly IBlogPostRepository blogPostRetriever;

        public BlogController(ILogger<BlogController> logger, IBlogPostRepository blogPostRetriever)
        {
            this.logger = logger;
            this.blogPostRetriever = blogPostRetriever;
        }

        public async Task<PostSummary[]> GetPostSummaries(CancellationToken token)
        {
            return await blogPostRetriever.GetPublishedPostSummaries(token);
        }

        public async Task<IActionResult> Index()
        {
            var summariesTask = GetPostSummaries(CancellationToken.None);
            var postsTask = blogPostRetriever.GetPublishedPosts(CancellationToken.None);

            return View("List", new BlogModel
            {
                Archive = await summariesTask,
                Posts = await postsTask
            });
        }

        public async Task<IActionResult> Posts(string id)
        {
            var singleTask = blogPostRetriever.GetPost(id, CancellationToken.None);

            var summariesTask = GetPostSummaries(CancellationToken.None);

            return View("Single", new BlogModel
            {
                Archive = await summariesTask,
                Single = await singleTask
            });
        }

        public async Task<IActionResult> Category(string id)
        {
            var summariesTask = GetPostSummaries(CancellationToken.None);
            var postsTask = blogPostRetriever.GetPostsForCategory(id, CancellationToken.None);

            return View("List", new BlogModel
            {
                Category = id,
                Archive = await summariesTask,
                Posts = await postsTask
            });
        }
    }
}