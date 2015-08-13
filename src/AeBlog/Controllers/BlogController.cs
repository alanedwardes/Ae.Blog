using AeBlog.Data;
using AeBlog.ViewModels;
using Microsoft.AspNet.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    [Route("/")]
    public class BlogController : Controller
    {
        private readonly IPostManager postManager;
        private readonly ILastfmDataProvider lastfmDataProvider;
        private readonly IPortfolioManager portfolioManager;
        private readonly ICategoryManager categoryManager;

        public BlogController(IPostManager postManager, IPortfolioManager portfolioManager, ILastfmDataProvider lastfmDataProvider, ICategoryManager categoryManager)
        {
            this.postManager = postManager;
            this.portfolioManager = portfolioManager;
            this.lastfmDataProvider = lastfmDataProvider;
            this.categoryManager = categoryManager;
        }

        [Route("/assets/uploads/{section}/{file}")]
        public IActionResult AssetsUploads(string section, string file)
        {
            return Redirect($"https://d2tboyarufvoyl.cloudfront.net/uploads/{section}/{file}");
        }

        [Route("/serve/{size}/{file}")]
        public IActionResult ServeLastfm(string size, string file)
        {
            return Redirect($"https://d2tboyarufvoyl.cloudfront.net/serve/{size}/{file}");
        }

        [Route("/")]
        public async Task<IActionResult> Home(CancellationToken ctx = default(CancellationToken))
        {
            var posts = await postManager.GetPublishedPosts(ctx);

            var portfolios = await portfolioManager.GetFeaturedPortfolios(ctx);

            var albums = await lastfmDataProvider.GetTopAlbumsForUser("alanedwardes", "", "7day", ctx);
            //var albums = Enumerable.Empty<Album>();

            return View(new HomeViewModel(posts, portfolios, albums));
        }

        [Route("/portfolio/")]
        public async Task<IActionResult> Portfolio(CancellationToken ctx = default(CancellationToken))
        {
            var portfolios = await portfolioManager.GetPublishedPortfolios(ctx);

            var categories = await categoryManager.GetPublishedCategories(ctx);

            return View(new PortfolioViewModel(portfolios, categories));
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
