using AeBlog.Caching;
using AeBlog.Clients;
using AeBlog.Data;
using AeBlog.Extensions;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class PostCacheTask : IScheduledTask
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IDynamoClientFactory dynamoclientFactory;
        private readonly string PostTable = "Aeblog.Post";

        public PostCacheTask(IDynamoClientFactory dynamoclientFactory, ICacheProvider cacheProvider)
        {
            this.dynamoclientFactory = dynamoclientFactory;
            this.cacheProvider = cacheProvider;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var client = dynamoclientFactory.CreateDynamoClient();

            var portfolioTable = Table.LoadTable(client, PostTable);

            var search = portfolioTable.Scan(new ScanFilter());

            var documents = new List<Document>();

            do
            {
                documents.AddRange(await search.GetNextSetAsync(ctx));
            }
            while (!search.IsDone);

            var portfolios = documents.Deserialize<Post>().ToList();

            await cacheProvider.Set("posts", portfolios, ctx);

            return TimeSpan.FromMinutes(5);
        }
    }
}
