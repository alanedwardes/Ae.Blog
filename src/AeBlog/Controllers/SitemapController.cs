using AeBlog.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AeBlog.Controllers
{
    [Route("/sitemap.xml")]
    public class SitemapController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;

        public SitemapController(IBlogPostRepository blogPostRepository)
        {
            this.blogPostRepository = blogPostRepository;
        }

        private const string XmlNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

        private XmlElement CreateUrl(XmlDocument document, string locText, string changeFreqText, string priorityText)
        {
            var url = document.CreateElement("url", XmlNamespace);

            var loc = document.CreateElement("loc", XmlNamespace);
            loc.InnerText = locText;
            url.AppendChild(loc);

            var changefreq = document.CreateElement("changefreq", XmlNamespace);
            changefreq.InnerText = changeFreqText;
            url.AppendChild(changefreq);

            var priority = document.CreateElement("priority", XmlNamespace);
            priority.InnerText = priorityText;
            url.AppendChild(priority);

            return url;
        }

        [HttpGet]
        public async Task<IActionResult> Sitemap()
        {
            const string baseUrl = "https://alanedwardes.com";

            var document = new XmlDocument();
            var xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.InsertBefore(xmlDeclaration, document.DocumentElement);

            var urlSet = document.CreateElement("urlset", XmlNamespace);
            document.AppendChild(urlSet);

            urlSet.AppendChild(CreateUrl(document, $"{baseUrl}/", "monthly", "1.0"));
            urlSet.AppendChild(CreateUrl(document, $"{baseUrl}/blog/", "weekly", "0.9"));
            urlSet.AppendChild(CreateUrl(document, $"{baseUrl}/projects/", "monthly", "0.8"));
            urlSet.AppendChild(CreateUrl(document, $"{baseUrl}/contact/", "monthly", "0.7"));

            var categories = new HashSet<string>();

            foreach (var post in await blogPostRepository.GetAllPostSummaries(CancellationToken.None))
            {
                urlSet.AppendChild(CreateUrl(document, $"{baseUrl}/blog/posts/{post.Slug}/", "monthly", "0.5"));
                categories.Add(post.Category);
            }

            foreach (var category in categories)
            {
                urlSet.AppendChild(CreateUrl(document, $"{baseUrl}/blog/category/{category}/", "daily", "0.4"));
            }

            var ms = new MemoryStream();
            document.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(ms, "text/xml");
        }
    }
}
