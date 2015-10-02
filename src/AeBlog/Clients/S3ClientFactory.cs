using Amazon.S3;
using Microsoft.Framework.OptionsModel;
using AeBlog.Options;
using Amazon.Runtime;
using Amazon;

namespace AeBlog.Clients
{
    public class S3ClientFactory : IS3ClientFactory
    {
        private readonly IOptions<Credentials> credentials;

        public S3ClientFactory(IOptions<Credentials> credentials)
        {
            this.credentials = credentials;
        }

        public IAmazonS3 CreateS3Client()
        {
            var awsCredentials = new BasicAWSCredentials(credentials.Options.AwsAccessKey, credentials.Options.AwsSecretKey);
            return new AmazonS3Client(awsCredentials, RegionEndpoint.GetBySystemName(credentials.Options.AwsRegion));
        }
    }
}
