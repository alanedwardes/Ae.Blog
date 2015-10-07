using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Framework.Logging;
using System;

namespace AeBlog.Logging
{
    public class SNSLogger : ILogger
    {
        private readonly IAmazonSimpleNotificationService client;
        private readonly string name;
        private readonly string topic;

        public SNSLogger(IAmazonSimpleNotificationService client, string name, string topic)
        {
            this.client = client;
            this.name = name;
            this.topic = topic;
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel > LogLevel.Information;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                // Don't await, and no ctx. We want to fire and forget, best-effort delivery
                client.PublishAsync(new PublishRequest(topic, formatter(state, exception), $"{logLevel.ToString()} in {name}"));
            }
        }
    }
}
