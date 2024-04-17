using System;
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
            var singleTask = blogPostRetriever.GetContent(id, CancellationToken.None);

            return View("Single", new BlogModel
            {
                Archive = await summariesTask,
                Single = await singleTask
            });
        }

        [HttpGet("/blog/posts/{id}.md")]
        public async Task<IActionResult> Markdown(string id)
        {
            var post = await blogPostRetriever.GetContent(id, CancellationToken.None);
            return Content(post.Content);
        }

        public async Task<IActionResult> Categories(string id)
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

        public async Task<IActionResult> Words(string id)
        {
            var posts = await blogPostRetriever.GetPublishedPosts(CancellationToken.None);

            var matchingPosts = new List<Post>();

            foreach (var post in posts)
            {
                if (post.ContentWordStatistics.ContainsKey(id))
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

        public async Task<JsonResult> Search()
        {
            var posts = await blogPostRetriever.GetPublishedPosts(CancellationToken.None);

            var words = new Dictionary<string, List<int>>();

            foreach (var post in posts)
            {
                var postIndex = Array.IndexOf(posts, post);

                var postWords = post.ContentWordStatistics;

                foreach (var word in postWords.Keys)
                {
                    if (!words.TryAdd(word, new List<int> { postIndex }))
                    {
                        words[word].Add(postIndex);
                    }
                }
            }

            return new JsonResult(new Dictionary<string, object>
            {
                { "posts", posts.Select(post => new { slug = post.Slug, title = post.Title }) },
                { "words", words.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value) }
            });
        }
    }
}