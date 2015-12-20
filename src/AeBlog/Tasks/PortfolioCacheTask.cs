using AeBlog.Data;
using AeBlog.Tasks.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class PortfolioCacheTask : ITask
    {
        private readonly IDocumentRetriever dynamoTableRetriever;

        public PortfolioCacheTask(IDocumentRetriever dynamoTableRetriever)
        {
            this.dynamoTableRetriever = dynamoTableRetriever;
        }

        public async Task DoWork(CancellationToken ctx)
        {
            await dynamoTableRetriever.RetrieveDocuments<Portfolio>(Portfolio.TableName, ctx);
        }
    }
}
