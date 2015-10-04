using AeBlog.Options;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Framework.OptionsModel;

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
            var awsCredentials = new BasicAWSCredentials(credentials.Options.AwsAccessKey, credentials.Options.AwsSecretKey);
            return new AmazonSQSClient(awsCredentials, RegionEndpoint.GetBySystemName(credentials.Options.AwsRegion));
        }
    }
}
