using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Services
{
    public class ImageRepository : IImageRepository
    {
        private readonly IAmazonDynamoDB dynamo;
        private readonly HttpClient client;

        public ImageRepository(IAmazonDynamoDB dynamo)
        {
            this.dynamo = dynamo;
            client = new HttpClient();
        }

        private string TableName => Environment.GetEnvironmentVariable("IMAGES_TABLE");

        public async Task<Tuple<int, int>> GetImageDimensions(string url, CancellationToken token)
        {
            var item = await dynamo.GetItemAsync(TableName, new Dictionary<string, AttributeValue>
            {
                {"Url", new AttributeValue(url)}
            }, token);

            if (item.Item.Any())
            {
                return Tuple.Create(int.Parse(item.Item["Width"].N), int.Parse(item.Item["Height"].N));
            }

            var image = Image.Load(await client.GetStreamAsync(url));
            await dynamo.PutItemAsync(TableName, new Dictionary<string, AttributeValue>
            {
                {"Url", new AttributeValue(url)},
                {"Width", new AttributeValue {N = image.Width.ToString()}},
                {"Height", new AttributeValue {N = image.Height.ToString()}},
            }, token);
            return Tuple.Create(image.Width, image.Height);
        }
    }
}
