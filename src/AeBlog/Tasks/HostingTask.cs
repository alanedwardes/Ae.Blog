using Microsoft.AspNet.Hosting;
using System;
using AeBlog.Options;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Configuration;

namespace AeBlog.Tasks
{
    public class HostingTask : IScheduledTask
    {
        private readonly IApplicationEnvironment environment;

        public HostingTask(IApplicationEnvironment environment)
        {
            this.environment = environment;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var configuration = new ConfigurationBuilder()
                .AddGlobalConfigSources()
                .Build();

            var builder = new WebHostBuilder(configuration.GetSection("Hosting"));
            var engine = builder.Build();

            using (engine.Start())
            {
                await Task.Delay(Timeout.Infinite, ctx);
            }

            return TimeSpan.Zero;
        }
    }
}
