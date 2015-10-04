using AeBlog.Clients;
using AeBlog.Extensions;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using System.Threading;
using System.Linq;
using AeBlog.Caching;

namespace AeBlog.Tasks.Helpers
{
    public class DynamoTableRetriever : IDynamoTableRetriever
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IAmazonDynamoDB dynamoClient;

        public DynamoTableRetriever(IDynamoClientFactory dynamoFactory, ICacheProvider cacheProvider)
        {
            dynamoClient = dynamoFactory.CreateDynamoClient();
            this.cacheProvider = cacheProvider;
        }

        public async Task RetrieveTable<TItem>(string tableName, CancellationToken ctx)
        {
            var portfolioTable = Table.LoadTable(dynamoClient, tableName);

            var search = portfolioTable.Scan(new ScanFilter());

            var documents = new List<Document>();

            do
            {
                documents.AddRange(await search.GetNextSetAsync(ctx));
            }
            while (!search.IsDone);

            var items = documents.Deserialize<TItem>().ToList();

            await cacheProvider.Set<IList<TItem>>(tableName, items, ctx);
        }
    }
}
