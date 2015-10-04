using AeBlog.Data;
using AeBlog.Tasks.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class PostCacheTask : IScheduledTask
    {
        private readonly IDynamoTableRetriever dynamoTableRetriever;

        public PostCacheTask(IDynamoTableRetriever dynamoTableRetriever)
        {
            this.dynamoTableRetriever = dynamoTableRetriever;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            await dynamoTableRetriever.RetrieveTable<Post>(Post.TableName, ctx);
            return TimeSpan.FromHours(1);
        }
    }
}
