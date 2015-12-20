using AeBlog.Data;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks.Helpers
{
    public class QueueMessageProcessor : IQueueMessageProcessor
    {
        private readonly IDynamoTableRetriever dynamoTableRetriever;

        private const string SourceARNKey = "eventSourceARN";
        private readonly ILogger<QueueMessageProcessor> logger;

        public QueueMessageProcessor(ILogger<QueueMessageProcessor> logger, IDynamoTableRetriever dynamoTableRetriever)
        {
            this.logger = logger;
            this.dynamoTableRetriever = dynamoTableRetriever;
        }

        public async Task ProcessMessage(Message message, CancellationToken ctx)
        {
            var eventSource = JToken.Parse(message.Body)[SourceARNKey].ToString();

            if (eventSource.Contains(Portfolio.TableName))
            {
                logger.LogInformation("Received event from SQS, refreshing portfolio table");
                await dynamoTableRetriever.RetrieveTable<Portfolio>(Portfolio.TableName, ctx);
            }
            else if (eventSource.Contains(Post.TableName))
            {
                logger.LogInformation("Received event from SQS, refreshing posts table");
                await dynamoTableRetriever.RetrieveTable<Post>(Post.TableName, ctx);
            }
            else
            {
                logger.LogError($"Unhandled message from {eventSource}");
            }
        }
    }
}
