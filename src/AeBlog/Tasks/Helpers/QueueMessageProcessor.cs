using AeBlog.Data;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks.Helpers
{
    public class QueueMessageProcessor : IQueueMessageProcessor
    {
        private readonly ILogger<QueueMessageProcessor> logger;
        private readonly IServiceProvider provider;

        public QueueMessageProcessor(ILogger<QueueMessageProcessor> logger, IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
        }

        public async Task ProcessMessage(Message message, CancellationToken ctx)
        {
            var type = typeof(QueueMessageProcessor).GetTypeInfo().Assembly.GetTypes()
                .Where(t => typeof(ITask).IsAssignableFrom(t) && t.GetTypeInfo().IsClass)
                .Where(t => t.Name == message.Body)
                .SingleOrDefault();

            if (type == null)
            {
                return;
            }
            else
            {
                var sw = new Stopwatch();
                var task = (ITask)ActivatorUtilities.CreateInstance(provider, type);
                logger.LogInformation($"Received event from SQS, running task {type.Name}");
                sw.Start();
                await task.DoWork(ctx);
                logger.LogInformation($"Triggered task {type.Name} completed in {sw.Elapsed.TotalSeconds} seconds");
            }
        }
    }
}
