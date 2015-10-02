using AeBlog.Caching;
using AeBlog.Data;
using AeBlog.Options;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class AlbumCacheTask : IScheduledTask
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IOptions<Credentials> credentials;
        private readonly ILastfmDataProvider lastFmDataProvider;

        public AlbumCacheTask(ILastfmDataProvider lastFmDataProvider, IOptions<Credentials> credentials, ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
            this.lastFmDataProvider = lastFmDataProvider;
            this.credentials = credentials;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var albums = await lastFmDataProvider.GetTopAlbumsForUser(credentials.Options.LastFmUsername, credentials.Options.LastFmApiKey, "7day", ctx);

            await cacheProvider.Set("albums", albums);

            return TimeSpan.FromHours(1);
        }
    }
}
