using AeBlog.Models;
using AeBlog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class AdminController : Controller
    {
        private readonly IBlogPostRetriever blogPostRetriever;

        public AdminController(IBlogPostRetriever blogPostRetriever)
        {
            this.blogPostRetriever = blogPostRetriever;
        }

        public async Task<IActionResult> Index()
        {
            var summaries = await blogPostRetriever.GetPostSummaries(CancellationToken.None);

            return View(new AdminModel
            {
                Posts = summaries
            });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var post = await blogPostRetriever.GetPost(id, CancellationToken.None);

            return View(new AdminModel
            {
                Post = post
            });
        }
    }
}