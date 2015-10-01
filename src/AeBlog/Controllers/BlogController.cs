using AeBlog.Data;
using AeBlog.Options;
using AeBlog.ViewModels;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    public class BlogController : Controller
    {
        private readonly IPostManager postManager;
        private readonly ILastfmDataProvider lastfmDataProvider;
        private readonly IPortfolioManager portfolioManager;
        private readonly ICacheProvider cacheProvider;

        public BlogController(IPostManager postManager, IPortfolioManager portfolioManager, ILastfmDataProvider lastfmDataProvider, ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
            this.postManager = postManager;
            this.portfolioManager = portfolioManager;
            this.lastfmDataProvider = lastfmDataProvider;
        }

        [Route("/assets/uploads/{section}/{file}")]
        public IActionResult AssetsUploads(string section, string file)
        {
            return Redirect($"https://d2tboyarufvoyl.cloudfront.net/uploads/{section}/{file}");
        }

        [Route("/")]
        public async Task<IActionResult> Home(CancellationToken ctx = default(CancellationToken))
        {
            var posts = await postManager.GetPublishedPosts(ctx);

            var portfolios = await portfolioManager.GetFeaturedPortfolios(ctx);

            var albums = await cacheProvider.Get<IEnumerable<Album>>("albums");

            return View(new HomeViewModel(posts, portfolios, albums));
        }

        [Route("/portfolio/")]
        public async Task<IActionResult> Portfolio(CancellationToken ctx = default(CancellationToken))
        {
            var portfolios = await portfolioManager.GetPublishedPortfolios(ctx);

            return View(new PortfolioViewModel(portfolios));
        }

        [Route("/portfolio/item/{id}/")]
        public async Task<IActionResult> SinglePortfolio(int id, CancellationToken ctx = default(CancellationToken))
        {
            var portfolio = await portfolioManager.GetPortfolioById(id, ctx);

            return View(new SinglePortfolioViewModel(portfolio));
        }

        [Route("/archive/")]
        public async Task<IActionResult> Archive(CancellationToken ctx = default(CancellationToken))
        {
            var posts = await postManager.GetPublishedPosts(ctx);

            return View(new ArchiveViewModel(posts));
        }

        [Route("/ext/")]
        public async Task<IActionResult> External()
        {
            string[] ValidDomains = new[] { "img2-ak.lst.fm" };

            var url = Request.Query.Get("url");

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return HttpNotFound();
            }

            if (!ValidDomains.Any(d => d == uri.Host))
            {
                return HttpNotFound();
            }

            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {
                var response = await client.GetAsync(uri);
                var contentType = response.Content.Headers.ContentType;
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return File(bytes, contentType.MediaType);
            }
        }

        [Route("/posts/{slug}/")]
        public async Task<IActionResult> SinglePost(string slug, CancellationToken ctx = default(CancellationToken))
        {
            var post = await postManager.GetPostBySlug(slug, ctx);
            if (post == null)
            {
                return HttpNotFound();
            }

            if (!post.IsPublished)
            {
                return HttpNotFound();
            }

            return View(new SinglePostViewModel(post));
        }

        [Route("/errors/{code}")]
        public IActionResult Error(int code)
        {
            switch (code)
            {
                case 404:
                    return View(new ErrorViewModel("Not Found", "Sorry, the page you were looking for wasn't found."));
                case 500:
                    return View(new ErrorViewModel("Internal Server Error", "Sorry, this page encountered a problem."));
                default:
                    return View(new ErrorViewModel($"Error {code}", "Sorry, this page encountered a problem."));
            }
        }
    }
}
