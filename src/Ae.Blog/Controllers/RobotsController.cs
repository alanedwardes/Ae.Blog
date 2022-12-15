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
                Content = string.Join("\n", new []
                {
                    "User-agent: *",
                    $"{permission}: /",
                    "Disallow: /blog/category/",
                    "Disallow: /blog/categories/",
                    "Disallow: /blog/word/",
                    "Disallow: /blog/words/",
                    "Sitemap: https://alanedwardes.com/sitemap.xml"
                }),
                ContentType = "text/plain"
            };
        }
    }
}
