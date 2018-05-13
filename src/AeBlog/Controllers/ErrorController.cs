using Microsoft.AspNetCore.Mvc;

namespace AeBlog.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}