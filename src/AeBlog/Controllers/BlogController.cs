using AeBlog.Caching;
using AeBlog.Clients;
using AeBlog.Data;
using AeBlog.Extensions;
using AeBlog.ViewModels;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    public class BlogController : Controller
    {
        private readonly IPostManager postManager;
        private readonly IPortfolioManager portfolioManager;
        private readonly ICacheProvider cacheProvider;

        public BlogController(IPostManager postManager, IPortfolioManager portfolioManager, ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
            this.postManager = postManager;
            this.portfolioManager = portfolioManager;
        }

        [Route("/assets/uploads/{section}/{file}")]
        public IActionResult AssetsUploads(string section, string file)
        {
            return Redirect($"https://d2tboyarufvoyl.cloudfront.net/uploads/{section}/{file}");
        }

        [Route("/")]
        public async Task<IActionResult> Home(CancellationToken ctx)
        {
            var posts = await postManager.GetPublishedPosts(ctx);

            var portfolios = await portfolioManager.GetFeaturedPortfolios(ctx);

            var albums = await cacheProvider.Get<IList<Album>>("albums", ctx) ?? Enumerable.Empty<Album>();

            return View(new HomeViewModel(posts, portfolios, albums));
        }

        [Route("/portfolio/")]
        public async Task<IActionResult> Portfolio(CancellationToken ctx)
        {
            var portfolios = await portfolioManager.GetPublishedPortfolios(ctx);

            return View(new PortfolioViewModel(portfolios));
        }

        [Route("/portfolio/item/{id}/")]
        public async Task<IActionResult> SinglePortfolio(int id, CancellationToken ctx)
        {
            var portfolio = await portfolioManager.GetPortfolioById(id, ctx);
            if (portfolio == null)
            {
                return HttpNotFound();
            }

            return View(new SinglePortfolioViewModel(portfolio));
        }

        [Route("/portfolio/skill/{slug}/")]
        public async Task<IActionResult> SingleSkill(string slug, CancellationToken ctx)
        {
            var portfolios = await portfolioManager.GetPortfoliosBySkillSlug(slug, ctx);
            if (!portfolios.Any())
            {
                return HttpNotFound();
            }

            var skill = portfolios.First().Skills.Single(s => s.ToSlug() == slug);

            return View(new SingleSkillViewModel(portfolios, skill));
        }

        [Route("/archive/")]
        public async Task<IActionResult> Archive(CancellationToken ctx)
        {
            var posts = await postManager.GetPublishedPosts(ctx);

            return View(new ArchiveViewModel(posts));
        }

        [Route("/posts/{slug}/")]
        public async Task<IActionResult> SinglePost(string slug, CancellationToken ctx)
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
