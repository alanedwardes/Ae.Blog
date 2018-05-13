using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            logger.LogInformation("Getting blog posts");
            var sw = Stopwatch.StartNew();
            var posts = await blogPostRetriever.GetPosts(CancellationToken.None);
            logger.LogInformation("Got blog posts in {0}ms", sw.ElapsedMilliseconds);
            return View(posts);
        }

        public async Task<IActionResult> Posts(string id)
        {
            var post = await blogPostRetriever.GetPost(id, CancellationToken.None);
            return View(post);
        }
    }
}