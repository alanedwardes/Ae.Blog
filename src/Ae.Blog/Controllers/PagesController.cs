using System.Threading;
using System.Threading.Tasks;
using Ae.Blog.Models;
using Ae.Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ae.Blog.Controllers
{
    public class PagesController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public PagesController(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public async Task<IActionResult> Page(string page, CancellationToken token)
        {
            var post = await _blogPostRepository.GetContent(page, token);

            return View(new PageModel
            {
                Title = post.Title,
                Content = post.Content
            });
        }
    }
}