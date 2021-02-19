using Microsoft.AspNetCore.Mvc;

namespace Ae.Blog.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
