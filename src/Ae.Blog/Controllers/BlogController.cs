using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ae.Blog.Extensions;
using Ae.Blog.Models;
using Ae.Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ae.Blog.Controllers
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

        public async Task<IActionResult> Index()
        {
            var postsTask = blogPostRetriever.GetPublishedPosts(CancellationToken.None);

            return View("List", new BlogModel
            {
                Archive = await postsTask,
                Posts = await postsTask
            });
        }

        public async Task<IActionResult> Posts(string id)
        {
            var summariesTask = blogPostRetriever.GetPublishedPosts(CancellationToken.None);
            var singleTask = blogPostRetriever.GetPost(id, CancellationToken.None);

            return View("Single", new BlogModel
            {
                Archive = await summariesTask,
                Single = await singleTask
            });
        }

        public async Task<IActionResult> Category(string id)
        {
            var posts = await blogPostRetriever.GetPublishedPosts(CancellationToken.None);

            return View("List", new BlogModel
            {
                FilterValue = id,
                FilterType = "Category",
                Archive = posts,
                Posts = posts.Where(x => x.Category == id).ToArray()
            });
        }

        public async Task<IActionResult> Word(string id)
        {
            var posts = await blogPostRetriever.GetPublishedPosts(CancellationToken.None);

            var matchingPosts = new List<Post>();

            foreach (var post in posts)
            {
                var words = new Dictionary<string, int>();
                post.GetWordStatistics(words);

                if (words.ContainsKey(id))
                {
                    matchingPosts.Add(post);
                }
            }

            return View("List", new BlogModel
            {
                FilterValue = id,
                FilterType = "Word",
                Archive = posts,
                Posts = matchingPosts
            });
        }
    }
}