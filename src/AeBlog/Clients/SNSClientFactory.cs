using AeBlog.Options;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.OptionsModel;

namespace AeBlog.Clients
{
    public class SNSClientFactory : ISNSClientFactory
    {
        private readonly IOptions<Credentials> credentials;

        public SNSClientFactory(IOptions<Credentials> credentials)
        {
            this.credentials = credentials;
        }

        public IAmazonSimpleNotificationService CreateSimpleNotificationClient()
        {
            var awsCredentials = new BasicAWSCredentials(credentials.Value.AwsAccessKey, credentials.Value.AwsSecretKey);
            return new AmazonSimpleNotificationServiceClient(awsCredentials, RegionEndpoint.GetBySystemName(credentials.Value.AwsRegion));
        }
    }
}
