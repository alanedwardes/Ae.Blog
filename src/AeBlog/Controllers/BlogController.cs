using System;
using System.Threading;
using System.Threading.Tasks;
using AeBlog.Models;
using AeBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AeBlog.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> logger;
        private readonly IBlogPostRepository blogPostRetriever;
        private readonly IMemoryCache memoryCache;

        public BlogController(ILogger<BlogController> logger, IBlogPostRepository blogPostRetriever, IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.blogPostRetriever = blogPostRetriever;
            this.memoryCache = memoryCache;
        }

        public async Task<PostSummary[]> GetPostSummaries(CancellationToken token)
        {
            return await memoryCache.GetOrCreateAsync("GetPublishedPostSummaries", entry => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return blogPostRetriever.GetPublishedPostSummaries(CancellationToken.None);
            });
        }

        public async Task<IActionResult> Index()
        {
            var summariesTask = GetPostSummaries(CancellationToken.None);
            var postsTask = memoryCache.GetOrCreateAsync("GetPublishedPosts", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return blogPostRetriever.GetPublishedPosts(CancellationToken.None);
            });

            return View("List", new BlogModel
            {
                Archive = await summariesTask,
                Posts = await postsTask
            });
        }

        public async Task<IActionResult> Posts(string id)
        {
            var summariesTask = GetPostSummaries(CancellationToken.None);
            var singleTask = memoryCache.GetOrCreateAsync("GetPost" + id, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return blogPostRetriever.GetPost(id, CancellationToken.None);
            });

            return View("Single", new BlogModel
            {
                Archive = await summariesTask,
                Single = await singleTask
            });
        }

        public async Task<IActionResult> Category(string id)
        {
            var summariesTask = GetPostSummaries(CancellationToken.None);
            var postsTask = memoryCache.GetOrCreateAsync("GetPostsForCategory" + id, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return blogPostRetriever.GetPostsForCategory(id, CancellationToken.None);
            });

            return View("List", new BlogModel
            {
                Category = id,
                Archive = await summariesTask,
                Posts = await postsTask
            });
        }
    }
}