using AeBlog.Data;
using Microsoft.AspNet.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    [Route("")]
    public class BlogController : Controller
    {
        private readonly IPostManager postManager;

        public BlogController(IPostManager postManager)
        {
            this.postManager = postManager;
        }

        [Route("/")]
        public IActionResult Index(CancellationToken ctx = default(CancellationToken))
        {
            return Content("It Works!");
        }

        [Route("/posts/{slug}/")]
        public async Task<IActionResult> SinglePost(string slug, CancellationToken ctx = default(CancellationToken))
        {
            var post = await postManager.GetPostBySlug(slug);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }
    }
}
