using Amazon.SQS;

namespace AeBlog.Clients
{
    public interface ISQSClientFactory
    {
        IAmazonSQS CreateSQSClient();
    }
}
