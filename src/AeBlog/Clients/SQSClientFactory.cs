using AeBlog.Options;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Extensions.OptionsModel;

namespace AeBlog.Clients
{
    public class SQSClientFactory : ISQSClientFactory
    {
        private readonly IOptions<Credentials> credentials;

        public SQSClientFactory(IOptions<Credentials> credentials)
        {
            this.credentials = credentials;
        }

        public IAmazonSQS CreateSQSClient()
        {
            var awsCredentials = new BasicAWSCredentials(credentials.Value.AwsAccessKey, credentials.Value.AwsSecretKey);
            return new AmazonSQSClient(awsCredentials, RegionEndpoint.GetBySystemName(credentials.Value.AwsRegion));
        }
    }
}
