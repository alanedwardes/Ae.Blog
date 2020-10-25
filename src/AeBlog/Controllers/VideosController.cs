using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Util;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Mvc;

namespace AeBlog.Controllers
{
    [Route("videos")]
    public class VideosController : Controller
    {
        private readonly YouTubeService youTubeService;

        public VideosController(YouTubeService youTubeService)
        {
            this.youTubeService = youTubeService;
        }

        private const string CHANNEL_ID = "UCVDebzker5ON8texg4S6STQ";

        public async Task<IActionResult> Index()
        {
            var videos = await GetVideos();
            var video = await GetVideo(videos.Items.First().Id.VideoId);
            return View((videos, video));
        }

        [Route("{videoId}")]
        public async Task<IActionResult> Single([FromRoute] string videoId)
        {
            return View("Index", (await GetVideos(), await GetVideo(videoId)));
        }

        private async Task<SearchListResponse> GetVideos()
        {
            var searchQuery = youTubeService.Search.List("snippet");
            searchQuery.ChannelId = CHANNEL_ID;
            searchQuery.MaxResults = 40;
            searchQuery.Order = SearchResource.ListRequest.OrderEnum.Date;
            return await searchQuery.ExecuteAsync();
        }

        private async Task<Video> GetVideo(string videoId)
        {
            var listQuery = youTubeService.Videos.List(new Repeatable<string>(new[] { "snippet", "contentDetails", "statistics" }));
            listQuery.Id = new Repeatable<string>(new[] { videoId });
            var response = await listQuery.ExecuteAsync();
            if (response.Items.Count == 0)
            {
                throw new Exception($"Video {videoId} not found");
            }
            var video = response.Items.Single();
            if (video.Snippet.ChannelId != CHANNEL_ID)
            {
                throw new Exception($"Video {videoId} doesn't belong to the correct channel");
            }
            return video;
        }
    }
}
