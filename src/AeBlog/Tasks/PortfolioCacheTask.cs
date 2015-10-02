using AeBlog.Caching;
using AeBlog.Clients;
using Amazon.DynamoDBv2.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class PortfolioCacheTask : IScheduledTask
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IDynamoClientFactory dynamoclientFactory;

        public PortfolioCacheTask(IDynamoClientFactory dynamoclientFactory, ICacheProvider cacheProvider)
        {
            this.dynamoclientFactory = dynamoclientFactory;
            this.cacheProvider = cacheProvider;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var client = dynamoclientFactory.CreateDynamoClient();

            var portfolios = await client.ScanAsync(new ScanRequest { TableName = "Aeblog.Portfolio" }, ctx);

            await cacheProvider.Set("portfolios", portfolios);

            return TimeSpan.FromMinutes(5);
        }
    }
}
