using AeBlog.Clients;
using AeBlog.Tasks.Helpers;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
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

        public TimeSpan Schedule => TimeSpan.FromSeconds(1);

        public QueuePollingTask(ILogger<QueuePollingTask> logger, IQueueMessageProcessor processor, ISQSClientFactory sqsClientFactory)
        {
            this.logger = logger;
            this.processor = processor;
            sqsClient = sqsClientFactory.CreateSQSClient();
        }

        public async Task DoWork(CancellationToken ctx)
        {
            var response = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest { QueueUrl = SQSQueueUrl }, ctx);

            if (!response.Messages.Any())
            {
                return;
            }

            logger.LogInformation($"Processing {response.Messages.Count} messages from SQS ({string.Join(", ", response.Messages.Select(m => m.MessageId))})");

            // Delete the messages first
            var deleteBatch = new DeleteMessageBatchRequest { QueueUrl = SQSQueueUrl };
            foreach (var message in response.Messages)
            {
                deleteBatch.Entries.Add(new DeleteMessageBatchRequestEntry(message.MessageId, message.ReceiptHandle));
            }
            await sqsClient.DeleteMessageBatchAsync(deleteBatch, ctx);

            // Then run all process tasks in parallel
            await Task.WhenAll(response.Messages.Select(m => processor.ProcessMessage(m, ctx)));
        }
    }
}
