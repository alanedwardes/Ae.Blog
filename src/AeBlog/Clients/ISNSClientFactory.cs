using Amazon.SimpleNotificationService;

namespace AeBlog.Clients
{
    public interface ISNSClientFactory
    {
        IAmazonSimpleNotificationService CreateSimpleNotificationClient();
    }
}