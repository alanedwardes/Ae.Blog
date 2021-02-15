using Microsoft.AspNetCore.Mvc;

namespace Ae.Blog.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}