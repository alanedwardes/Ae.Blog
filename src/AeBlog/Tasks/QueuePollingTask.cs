using AeBlog.Clients;
using AeBlog.Tasks.Helpers;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Framework.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class QueuePollingTask : IScheduledTask
    {
        private readonly ILogger<QueuePollingTask> logger;
        private readonly IQueueMessageProcessor processor;
        private readonly IAmazonSQS sqsClient;

        private const string SQSQueueUrl = "https://sqs.eu-west-1.amazonaws.com/687908690092/AeBlogMessages";

        public QueuePollingTask(ILogger<QueuePollingTask> logger, IQueueMessageProcessor processor, ISQSClientFactory sqsClientFactory)
        {
            this.logger = logger;
            this.processor = processor;
            sqsClient = sqsClientFactory.CreateSQSClient();
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var response = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = SQSQueueUrl
            }, ctx);

            if (!response.Messages.Any())
            {
                return TimeSpan.Zero;
            }

            logger.LogInformation($"Processing {response.Messages.Count} messages from SQS");

            // Delete the messages first
            var deleteBatch = new DeleteMessageBatchRequest { QueueUrl = SQSQueueUrl };
            foreach (var message in response.Messages)
            {
                deleteBatch.Entries.Add(new DeleteMessageBatchRequestEntry(message.MessageId, message.ReceiptHandle));
            }
            await sqsClient.DeleteMessageBatchAsync(deleteBatch, ctx);

            var processTasks = response.Messages.Select(m => processor.ProcessMessage(m, ctx));

            // Then run all process tasks in parallel
            await Task.WhenAll(processTasks);

            // If SQS responds immediately
            // for some reason, delay it a bit
            // to stop crazy CPU usage
            return TimeSpan.FromMilliseconds(500);
        }
    }
}
