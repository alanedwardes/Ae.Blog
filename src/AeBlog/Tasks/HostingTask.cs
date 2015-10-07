using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using System;
using AeBlog.Options;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class HostingTask : IScheduledTask
    {
        private readonly IApplicationEnvironment environment;
        private readonly IServiceProvider serviceProvider;

        public HostingTask(IServiceProvider serviceProvider, IApplicationEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            this.environment = environment;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var configuration = new ConfigurationBuilder(environment.ApplicationBasePath)
                .AddGlobalConfigSources()
                .Build();

            var builder = new WebHostBuilder(serviceProvider, configuration.GetSection("Hosting"));
            var engine = builder.Build();

            using (engine.Start())
            {
                await Task.Delay(Timeout.Infinite, ctx);
            }

            return TimeSpan.Zero;
        }
    }
}
