using Amazon.SQS.Model;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks.Helpers
{
    public interface IQueueMessageProcessor
    {
        Task ProcessMessage(Message message, CancellationToken ctx);
    }
}
