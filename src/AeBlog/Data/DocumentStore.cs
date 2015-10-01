using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Framework.Logging;
using System.IO;
using AeBlog.Clients;

namespace AeBlog.Data
{
    public class DocumentStore : IDocumentStore
    {
        private readonly IAmazonDynamoDB dynamo;
        private readonly ILogger<DocumentStore> logger;

        public DocumentStore(ILogger<DocumentStore> logger, IDynamoClientFactory dynamoClientFactory)
        {
            this.logger = logger;
            this.dynamo = dynamoClientFactory.CreateDynamoClient();
        }

        public TType DocumentToType<TType>(Document document) where TType : class
        {
            if (document == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<TType>(document.ToJson());
            }
            catch (JsonSerializationException)
            {
                logger.LogError($"Unable to deserialise document into {typeof(TType).Name}:\n{document.ToJsonPretty()}");
                return null;
            }
        }

        public Document TypeToDocument<TItem>(TItem type) where TItem : class
        {
            return Document.FromJson(JsonConvert.SerializeObject(type));
        }

        public async Task StoreBinaryItem<TItem>(string tableName, TItem item, string key, byte[] bytes) where TItem : class
        {
            var document = TypeToDocument(item);
            document.Add(key, new Primitive { Type = DynamoDBEntryType.Binary, Value = bytes });
            var table = Table.LoadTable(dynamo, tableName);
            await table.PutItemAsync(document);
        }

        public async Task<TItem> GetBinaryItem<TItem>(string tableName, string key, object value, string binaryKey, Stream stream) where TItem : class
        {
            Primitive primitive;
            if (value is int)
            {
                primitive = new Primitive(value.ToString(), true);
            }
            else
            {
                primitive = new Primitive(value.ToString(), false);
            }

            var table = Table.LoadTable(dynamo, tableName);
            var document = await table.GetItemAsync(new Dictionary<string, DynamoDBEntry>
            {
                { key, primitive } 
            });

            if (document == null)
            {
                return null;
            }

            DynamoDBEntry entry;
            if (!document.TryGetValue(binaryKey, out entry))
            {
                return null;
            }

            var bytes = entry.AsByteArray();
            await stream.WriteAsync(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            document.Remove(binaryKey);

            return DocumentToType<TItem>(document);
        }

        public async Task<IEnumerable<TItem>> GetItems<TItem>(string tableName, CancellationToken ctx = default(CancellationToken)) where TItem : class
        {
            logger.LogVerbose($"Getting all {typeof(TItem).Name} from {tableName}");

            var table = Table.LoadTable(dynamo, tableName);

            var query = table.Scan(new ScanOperationConfig());

            IEnumerable<TItem> result = new List<TItem>();

            var items = await PerformSearch(query, ctx);

            return items.Select(DocumentToType<TItem>).Where(i => i != null);
        }

        public async Task<IEnumerable<TItem>> GetItems<TItem>(string tableName, string key, object value, string index, CancellationToken ctx = default(CancellationToken)) where TItem : class
        {
            logger.LogVerbose($"Getting {typeof(TItem).Name} with the key {key} from {tableName}");

            var table = Table.LoadTable(dynamo, tableName);

            QueryFilter queryFilter;
            if (value is int || value is long)
            {
                queryFilter = new QueryFilter(key, QueryOperator.Equal, (int)value);
            }
            else
            {
                queryFilter = new QueryFilter(key, QueryOperator.Equal, (string)value);
            }

            var op = new QueryOperationConfig { IndexName = index, Filter = queryFilter };

            var search = table.Query(op);

            var items = await PerformSearch(search, ctx);
            return items.Select(DocumentToType<TItem>).Where(i => i != null);
        }

        public async Task<IEnumerable<Document>> PerformSearch(Search search, CancellationToken ctx = default(CancellationToken))
        {
            var result = new List<Document>();

            do
            {
                var items = await search.GetNextSetAsync(ctx);
                result.AddRange(items);
            }
            while (!search.IsDone);

            return result;
        }
    }
}
