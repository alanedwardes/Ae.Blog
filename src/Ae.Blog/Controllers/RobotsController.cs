using Microsoft.AspNetCore.Mvc;

namespace Ae.Blog.Controllers
{
    [Route("/robots.txt")]
    public class RobotsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Robots()
        {
            string permission = Request.Headers.ContainsKey("Freezing") ? "Allow" : "Disallow";

            return new ContentResult
            {
                Content = $"User-agent: *\n{permission}: /\nDisallow: /blog/category/\nDisallow: /blog/word/\nSitemap: /sitemap.xml",
                ContentType = "text/plain"
            };
        }
    }
}
