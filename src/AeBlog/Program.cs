using Microsoft.Dnx.Runtime;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using AeBlog.Options;
using Microsoft.Framework.Configuration;
using System;
using Microsoft.Framework.Logging;
using System.Threading;

namespace AeBlog
{
    public class Program
    {
        private readonly IServiceProvider defaultProvider;
        private readonly IApplicationEnvironment environment;

        public Program(IApplicationEnvironment environment, IServiceProvider defaultProvider)
        {
            this.defaultProvider = defaultProvider;
            this.environment = environment;
        }

        public async Task Main()
        {
            var credentials = new ConfigurationBuilder(environment.ApplicationBasePath)
                .AddJsonFile("credentials.json")
                .AddEnvironmentVariables("AeBlog")
                .Build();

            var services = new ServiceCollection();
            services.AddDefaultServices(defaultProvider);
            services.AddAssembly(typeof(Program).GetTypeInfo().Assembly);
            services.Configure<Credentials>(credentials);
            services.AddOptions();
            services.AddSingleton(x => MemoryCache.INSTANCE);
            services.AddLogging();

            var provider = services.BuildServiceProvider();

            var logging = provider.GetService<ILoggerFactory>();
            logging.AddConsole();

            var tasks = new TaskRunner().RunTasksFromAssembly(typeof(Program).GetTypeInfo().Assembly, provider, default(CancellationToken));

            await Task.WhenAll(tasks);
        }
    }
}
