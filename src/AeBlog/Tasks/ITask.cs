using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public interface ITask
    {
        Task DoWork(CancellationToken ctx);
    }
}
