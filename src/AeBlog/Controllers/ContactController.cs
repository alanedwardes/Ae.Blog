using Microsoft.AspNetCore.Mvc;

namespace AeBlog.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}