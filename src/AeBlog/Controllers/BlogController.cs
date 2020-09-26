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
            return await GetOrCreateAsync("GetPublishedPostSummaries", expiry => {
                expiry.Time = TimeSpan.FromHours(1);
                return blogPostRetriever.GetPublishedPostSummaries(token);
            });
        }

        public class CacheExpiry
        {
            public TimeSpan Time { get; set; }
        }

        private async Task<TItem> GetOrCreateAsync<TItem>(object key, Func<CacheExpiry, Task<TItem>> factory)
        {
            if (User.Identity.IsAuthenticated)
            {
                return await factory(new CacheExpiry());
            }

            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                var expiry = new CacheExpiry();
                var item = await factory(expiry);
                entry.AbsoluteExpirationRelativeToNow = expiry.Time;
                return item;
            });
        }

        public async Task<IActionResult> Index()
        {
            var summariesTask = GetPostSummaries(CancellationToken.None);
            var postsTask = GetOrCreateAsync("GetPublishedPosts", expiry =>
            {
                expiry.Time = TimeSpan.FromMinutes(1);
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
            var singleTask = GetOrCreateAsync("GetPost" + id, expiry =>
            {
                expiry.Time = TimeSpan.FromMinutes(1);
                return blogPostRetriever.GetPost(id, CancellationToken.None);
            });

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
            var postsTask = GetOrCreateAsync("GetPostsForCategory" + id, expiry =>
            {
                expiry.Time = TimeSpan.FromMinutes(1);
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