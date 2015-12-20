using System.Threading.Tasks;
using System.Reflection;
using AeBlog.Options;
using System;
using System.Threading;
using AeBlog.Tasks;
using AeBlog.Caching;
using AeBlog.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            var services = new ServiceCollection();
            services.AddDefaultServices(defaultProvider);
            services.AddAssembly(typeof(Program).GetTypeInfo().Assembly);
            services.AddOptions();
            services.AddSingleton(x => MemoryCache.INSTANCE);
            services.AddLogging();
            services.AddGlobalConfiguration(environment);

            var provider = services.BuildServiceProvider();

            var logging = provider.GetService<ILoggerFactory>();
            logging.AddProvider(provider.GetService<ISNSLoggerProvider>());
            logging.AddConsole();

            var cancellationSource = new CancellationTokenSource();

            var taskRunner = new TaskRunner(logging.CreateLogger<TaskRunner>(), provider);

            var tasks = taskRunner.RunTasksFromAssembly(typeof(Program).GetTypeInfo().Assembly, cancellationSource.Token);

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Caught cancel, exiting...");
                cancellationSource.Cancel();
            };

            await Task.WhenAll(tasks);
        }
    }
}
