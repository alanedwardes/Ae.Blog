using Microsoft.AspNetCore.Mvc;

namespace Ae.Blog.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}