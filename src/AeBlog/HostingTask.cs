using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog
{
    public class HostingTask : IScheduledTask
    {
        private readonly IServiceProvider serviceProvider;

        public HostingTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            return Task.Run(() => {
                new Microsoft.AspNet.Hosting.Program(serviceProvider).Main(new[] { "--config", "hosting.ini" });
                return TimeSpan.FromSeconds(0);
            });
        }
    }
}
