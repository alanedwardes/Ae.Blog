using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;

namespace AeBlog.Controllers
{
    public class VideosController : Controller
    {
        private readonly YouTubeService youTubeService;

        public VideosController(YouTubeService youTubeService)
        {
            this.youTubeService = youTubeService;
        }

        public async Task<IActionResult> Index()
        {
            var searchQuery = youTubeService.Search.List("snippet");
            searchQuery.ChannelId = "UCVDebzker5ON8texg4S6STQ";
            searchQuery.MaxResults = 20;
            searchQuery.Order = SearchResource.ListRequest.OrderEnum.Date;
            return View(await searchQuery.ExecuteAsync());
        }
    }
}
