using AeBlog.Data;
using AeBlog.Options;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog
{
    public class LastfmAlbumCacheService : IScheduledTask
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IOptions<Credentials> credentials;
        private readonly ILastfmDataProvider lastFmDataProvider;
        private readonly ILogger<LastfmAlbumCacheService> logger;

        public LastfmAlbumCacheService(ILastfmDataProvider lastFmDataProvider, IOptions<Credentials> credentials, ICacheProvider cacheProvider, ILogger<LastfmAlbumCacheService> logger)
        {
            this.logger = logger;
            this.cacheProvider = cacheProvider;
            this.lastFmDataProvider = lastFmDataProvider;
            this.credentials = credentials;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            logger.LogInformation("Fetching Last.fm albums.");
            var sw = new Stopwatch();

            sw.Start();
            var albums = await lastFmDataProvider.GetTopAlbumsForUser(credentials.Options.LastFmUsername, credentials.Options.LastFmApiKey, "7day", ctx);
            sw.Stop();

            logger.LogInformation($"Got albums. Took {sw.Elapsed}");

            sw.Restart();
            await cacheProvider.Set("albums", albums);
            sw.Stop();

            logger.LogInformation($"Cached albums. Took {sw.Elapsed}");

            return TimeSpan.FromMinutes(1);
        }
    }
}
