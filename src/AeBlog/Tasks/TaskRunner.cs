using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            var scheduledTasks = assembly.GetTypes().Where(t => typeof(ITask).IsAssignableFrom(t) && t.GetTypeInfo().IsClass);
            foreach (var type in scheduledTasks)
            {
                yield return Task.Run(async () => await WorkTask(type, ctx));
            }
        }

        public async Task WorkTask(Type type, CancellationToken ctx)
        {
            logger.LogInformation($"Started task {type.FullName} in thread {Thread.CurrentThread.ManagedThreadId}");
            do
            {
                var sw = new Stopwatch();
                sw.Start();
                var task = (ITask)ActivatorUtilities.CreateInstance(provider, type);
                try
                {
                    await task.DoWork(ctx);
                    var schedule = task as IScheduledTask;
                    if (schedule == null)
                    {
                        logger.LogInformation($"Task {type.Name} completed in {Math.Round(sw.Elapsed.TotalSeconds, 2)} seconds");
                        return;
                    }
                    logger.LogInformation($"Task {type.Name} completed in {Math.Round(sw.Elapsed.TotalSeconds, 2)} seconds. Next run in {Math.Round(schedule.Schedule.TotalMinutes, 2)} minutes");
                    await Task.Delay(schedule.Schedule);
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
