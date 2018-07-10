using Microsoft.AspNetCore.Mvc;

namespace AeBlog.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}