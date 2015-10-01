using AeBlog.Options;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Framework.OptionsModel;

namespace AeBlog.Clients
{
    public class DynamoClientFactory : IDynamoClientFactory
    {
        private readonly IOptions<Credentials> credentials;

        public DynamoClientFactory(IOptions<Credentials> credentials)
        {
            this.credentials = credentials;
        }

        public IAmazonDynamoDB CreateDynamoClient()
        {
            var awsCredentials = new BasicAWSCredentials(credentials.Options.AwsAccessKey, credentials.Options.AwsSecretKey);
            return new AmazonDynamoDBClient(awsCredentials, RegionEndpoint.GetBySystemName(credentials.Options.AwsRegion));
        }
    }
}
