using AeBlog.Models;
using AeBlog.Models.Admin;
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

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditPostModel model)
        {
            var post = await blogPostRetriever.GetPost(id, CancellationToken.None);

            post.Content = new PostContent { Markdown = model.Content };
            post.Category = model.Category;
            post.Type = model.IsPublished ? "published" : "draft";
            post.Title = model.Title;

            await blogPostRetriever.PutPost(post, CancellationToken.None);

            return Redirect("/admin/");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var post = await blogPostRetriever.GetPost(id, CancellationToken.None);

            return View(new EditPostModel
            {
                Title = post.Title,
                Category = post.Category,
                Content = post.Content.Markdown,
                IsPublished = post.Type == "published"
            });
        }
    }
}