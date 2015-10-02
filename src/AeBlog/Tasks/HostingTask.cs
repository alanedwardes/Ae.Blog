using Microsoft.AspNet.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class HostingTask : IScheduledTask
    {
        private readonly IServiceProvider serviceProvider;

        public HostingTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var builder = new WebHostBuilder(serviceProvider);
            builder.UseServer("Microsoft.AspNet.Server.Kestrel");
            var engine = builder.Build();

            using (engine.Start())
            {
                await Task.Delay(Timeout.Infinite, ctx);
            }

            return TimeSpan.Zero;
        }
    }
}
