using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog
{
    public interface IScheduledTask
    {
        Task<TimeSpan> DoWork(CancellationToken ctx);
    }
}
