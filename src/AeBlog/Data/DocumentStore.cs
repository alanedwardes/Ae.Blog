using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public class DocumentStore : IDocumentStore
    {
        private readonly IAmazonDynamoDB dynamo;

        public DocumentStore()
        {
            dynamo = new AmazonDynamoDBClient(new EnvironmentAWSCredentials(), Amazon.RegionEndpoint.EUWest1);
        }

        public TDocument DocumentToType<TDocument>(Document document) where TDocument : class
        {
            if (document == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<TDocument>(document.ToJson());
            }
            catch (JsonSerializationException)
            {
                return null;
            }
        }

        public async Task<TItem> GetItemAsync<TItem>(string key, string tableName, CancellationToken ctx = default(CancellationToken)) where TItem : class
        {
            var table = Table.LoadTable(dynamo, tableName);
            var item = await table.GetItemAsync(new Primitive(key), ctx);
            return DocumentToType<TItem>(item);
        }
    }
}
