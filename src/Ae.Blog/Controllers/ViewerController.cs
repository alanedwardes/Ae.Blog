using Microsoft.AspNetCore.Mvc;

namespace Ae.Blog.Controllers
{
    public class ViewerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
