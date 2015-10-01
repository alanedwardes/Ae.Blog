using Amazon.DynamoDBv2;

namespace AeBlog.Clients
{
    public interface IDynamoClientFactory
    {
        IAmazonDynamoDB CreateDynamoClient();
    }
}