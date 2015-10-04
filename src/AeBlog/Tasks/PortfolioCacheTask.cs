using AeBlog.Data;
using AeBlog.Tasks.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class PortfolioCacheTask : IScheduledTask
    {
        private readonly IDynamoTableRetriever dynamoTableRetriever;

        public PortfolioCacheTask(IDynamoTableRetriever dynamoTableRetriever)
        {
            this.dynamoTableRetriever = dynamoTableRetriever;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            await dynamoTableRetriever.RetrieveTable<Portfolio>(Portfolio.TableName, ctx);
            return TimeSpan.FromHours(1);
        }
    }
}
