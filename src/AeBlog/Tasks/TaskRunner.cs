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

        public IEnumerable<Task> RunTasksFromAssembly(Assembly assembly, CancellationToken ctx)
        {
            var scheduledTasks = assembly.GetTypes().Where(t => typeof(IScheduledTask).IsAssignableFrom(t) && t.GetTypeInfo().IsClass);
            foreach (var type in scheduledTasks)
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
                    logger.LogInformation($"Task {type.Name} completed in {sw.Elapsed.TotalSeconds} seconds. Next run: {DateTime.Now + delay}");
                    await Task.Delay(delay, ctx);
                }
                catch (Exception ex)
                {
                    if (ex is TaskCanceledException && ctx.IsCancellationRequested)
                    {
                        throw;
                    }
                    logger.LogError($"Exception from {type.Name} in {sw.Elapsed.TotalSeconds} seconds. Trying again in 30 seconds.", ex);
                    await Task.Delay(TimeSpan.FromSeconds(30), ctx);
                }
            } while (true);
        }
    }
}
