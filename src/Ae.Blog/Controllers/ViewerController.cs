using Microsoft.AspNetCore.Mvc;
using System;

namespace Ae.Blog.Controllers
{
    public class ViewerController : Controller
    {
        public IActionResult Model(Guid id)
        {
            return View(new Uri("https://s.edward.es/" + id + ".glb"));
        }
    }
}
