using AeBlog.Caching;
using AeBlog.Clients;
using Amazon.DynamoDBv2.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class PostCacheTask : IScheduledTask
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IDynamoClientFactory dynamoclientFactory;

        public PostCacheTask(IDynamoClientFactory dynamoclientFactory, ICacheProvider cacheProvider)
        {
            this.dynamoclientFactory = dynamoclientFactory;
            this.cacheProvider = cacheProvider;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var client = dynamoclientFactory.CreateDynamoClient();

            var posts = await client.ScanAsync(new ScanRequest { TableName = "Aeblog.Post" }, ctx);

            await cacheProvider.Set("posts", posts);

            return TimeSpan.FromMinutes(5);
        }
    }
}
