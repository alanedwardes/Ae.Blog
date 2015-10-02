using Amazon.S3;

namespace AeBlog.Clients
{
    public interface IS3ClientFactory
    {
        IAmazonS3 CreateS3Client();
    }
}
