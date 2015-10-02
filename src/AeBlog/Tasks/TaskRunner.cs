using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class TaskRunner
    {
        private readonly ILogger logger;
        private readonly IServiceProvider provider;

        public TaskRunner(ILogger logger, IServiceProvider provider)
        {
            this.provider = provider;
            this.logger = logger;
        }

        private IEnumerable<Type> GetScheduledTaskTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => typeof(IScheduledTask).IsAssignableFrom(t) && t.GetTypeInfo().IsClass);
        }

        public IEnumerable<Task> RunTasksFromAssembly(Assembly assembly, CancellationToken ctx)
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

            foreach (var type in GetScheduledTaskTypes(assembly))
            {
                yield return WorkTask(type, ctx);
            }
        }

        public async Task WorkTask(Type type, CancellationToken ctx)
        {
            do
            {
                logger.LogInformation($"Performing task {type.Name}");
                var sw = new Stopwatch();
                sw.Start();
                var scheduler = (IScheduledTask)ActivatorUtilities.CreateInstance(provider, type);
                try
                {
                    var delay = await scheduler.DoWork(ctx);
                    sw.Stop();
                    logger.LogInformation($"Task {type.Name} completed in {sw.Elapsed.TotalSeconds} seconds. Next run: {DateTime.Now + delay}");
                    await Task.Delay(delay, ctx);
                }
                catch (Exception ex)
                {
                    if (ex is TaskCanceledException && ctx.IsCancellationRequested)
                    {
                        throw;
                    }

                    sw.Stop();
                    logger.LogError($"Exception from {type.Name} in {sw.Elapsed.TotalSeconds} seconds. Trying again in 30 seconds.", ex);
                    await Task.Delay(TimeSpan.FromSeconds(30), ctx);
                }
            } while (true);
        }
    }
}
