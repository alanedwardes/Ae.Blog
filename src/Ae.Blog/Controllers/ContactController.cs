using Microsoft.AspNetCore.Mvc;

namespace Ae.Blog.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}