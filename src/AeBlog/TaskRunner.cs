using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog
{
    public class TaskRunner
    {
        private IEnumerable<Type> GetScheduledTaskTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => typeof(IScheduledTask).IsAssignableFrom(t) && t.GetTypeInfo().IsClass);
        }

        public IEnumerable<Task> RunTasksFromAssembly(Assembly assembly, IServiceProvider provider, CancellationToken ctx)
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

            foreach (var type in GetScheduledTaskTypes(assembly))
            {
                var logger = loggerFactory.CreateLogger<TaskRunner>();

                yield return Task.Run(async () =>
                {
                    while (true)
                    {
                        var scheduler = (IScheduledTask)ActivatorUtilities.CreateInstance(provider, type);
                        try
                        {
                            logger.LogInformation($"Performing task {type.Name}");
                            var delay = await scheduler.DoWork(ctx);
                            logger.LogInformation($"Task {type.Name} successful. Next performing in {delay}");
                            await Task.Delay(delay, ctx);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError("Exception from scheduled task. Trying again in 30 seconds.", ex);
                            await Task.Delay(TimeSpan.FromSeconds(30), ctx);
                        }
                    }
                });
            }
        }
    }
}
