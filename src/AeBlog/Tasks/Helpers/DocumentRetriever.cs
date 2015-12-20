using AeBlog.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using AeBlog.Caching;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.OptionsModel;
using AeBlog.Options;

namespace AeBlog.Tasks.Helpers
{
    public class DocumentRetriever : IDocumentRetriever
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IOptions<General> general;
        private readonly IAmazonS3 s3Client;

        public DocumentRetriever(IS3ClientFactory s3Factory, ICacheProvider cacheProvider, IOptions<General> general)
        {
            s3Client = s3Factory.CreateS3Client();
            this.cacheProvider = cacheProvider;
            this.general = general;
        }

        public async Task RetrieveDocuments<TItem>(string prefix, CancellationToken ctx)
        {
            var keys = await s3Client.ListObjectsAsync(new ListObjectsRequest {
                BucketName = general.Value.DocumentBucket,
                Prefix = prefix
            });

            var tasks = keys.S3Objects.Where(x => x.Key.EndsWith(".json")).Select(x => s3Client.GetObjectAsync(new GetObjectRequest {
                BucketName = general.Value.DocumentBucket,
                Key = x.Key,
            }, ctx));

            await Task.WhenAll(tasks);

            var items = new List<TItem>();

            foreach (var item in tasks.Select(t => t.Result))
            {
                using (var reader = new StreamReader(item.ResponseStream))
                using (var tr = new JsonTextReader(reader))
                {
                    items.Add(new JsonSerializer().Deserialize<TItem>(tr));
                }
            }

            await cacheProvider.Set<IList<TItem>>(prefix, items, ctx);
        }
    }
}
