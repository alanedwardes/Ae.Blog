using AeBlog.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AeBlog.Controllers
{
    [Route("/rss.xml")]
    public class RssController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;

        public RssController(IBlogPostRepository blogPostRepository)
        {
            this.blogPostRepository = blogPostRepository;
        }

        private XmlElement CreateElement(XmlDocument document, string name, string innerText)
        {
            var element = document.CreateElement(name);
            element.InnerText = innerText;
            return element;
        }
        
        [HttpGet]
        public async Task<IActionResult> Rss()
        {
            var document = new XmlDocument();
            var xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.InsertBefore(xmlDeclaration, document.DocumentElement);

            var rss = document.CreateElement("rss");
            rss.SetAttribute("version", "2.0");
            rss.SetAttribute("xmlns:atom", "http://www.w3.org/2005/Atom");

            document.AppendChild(rss);

            var channel = document.CreateElement("channel");
            rss.AppendChild(channel);

            var atomLink = document.CreateElement("atom", "link", "http://www.w3.org/2005/Atom");
            atomLink.SetAttribute("href", "https://alanedwardes.com/rss.xml");
            atomLink.SetAttribute("rel", "self");
            atomLink.SetAttribute("type", "application/rss+xml");
            channel.AppendChild(atomLink);

            var posts = (await blogPostRepository.GetPublishedPostSummaries(CancellationToken.None)).OrderByDescending(x => x.Published).ToArray();

            channel.AppendChild(CreateElement(document, "title", "Alan Edwardes"));
            channel.AppendChild(CreateElement(document, "description", "Blog posts for Alan Edwardes' personal blog."));
            channel.AppendChild(CreateElement(document, "link", "https://alanedwardes.com/"));
            channel.AppendChild(CreateElement(document, "lastBuildDate", DateTime.UtcNow.ToString("R")));
            channel.AppendChild(CreateElement(document, "pubDate", posts[0].Published.ToString("R")));
            channel.AppendChild(CreateElement(document, "ttl", "1440"));

            foreach (var post in posts)
            {
                var item = document.CreateElement("item");
                channel.AppendChild(item);

                item.AppendChild(CreateElement(document, "title", post.Title));
                item.AppendChild(CreateElement(document, "link", $"https://alanedwardes.com/blog/posts/{post.Slug}/"));
                var guidElement = CreateElement(document, "guid", post.Slug);
                guidElement.SetAttribute("isPermaLink", "false");
                item.AppendChild(guidElement);
                item.AppendChild(CreateElement(document, "pubDate", post.Published.ToString("R")));
            }

            var ms = new MemoryStream();
            document.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(ms, "application/rss+xml");
        }
    }
}
