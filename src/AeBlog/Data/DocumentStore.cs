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

namespace AeBlog.Data
{
    public class DocumentStore : IDocumentStore
    {
        private readonly IAmazonDynamoDB dynamo;
        private readonly ILogger<DocumentStore> logger;

        public DocumentStore(ILogger<DocumentStore> logger)
        {
            this.logger = logger;
            dynamo = new AmazonDynamoDBClient(new EnvironmentAWSCredentials(), RegionEndpoint.EUWest1);
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
                logger.LogError($"Unable to deserialise document into {typeof(TDocument).Name}:\n{document.ToJsonPretty()}");
                return null;
            }
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
