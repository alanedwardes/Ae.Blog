using AeBlog.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Services
{

    public class BlogPostRetriever : IBlogPostRetriever
    {
        private readonly IAmazonDynamoDB amazonDynamoDb;
        private readonly IAmazonS3 amazonS3;

        public BlogPostRetriever(IAmazonDynamoDB amazonDynamoDb, IAmazonS3 amazonS3)
        {
            this.amazonDynamoDb = amazonDynamoDb;
            this.amazonS3 = amazonS3;
        }

        public async Task<PostSummary> GetPost(string slug, CancellationToken token)
        {
            var metaTask = amazonDynamoDb.GetItemAsync("BlogPosts", new Dictionary<string, AttributeValue>
            {
                { "Slug", new AttributeValue(slug) }
            }, token);

            async Task<string> getContent()
            {
                var response = await amazonS3.GetObjectStreamAsync("ae-posts", slug + ".md", null, token);
                using (var sr = new StreamReader(response))
                {
                    return await sr.ReadToEndAsync();
                }
            };

            var getContentTask = getContent();

            var post = (await metaTask).Item;
            var content = await getContentTask;
            return new PostSummary
            {
                Category = post["Category"].S,
                Published = DateTime.Parse(post["Published"].S),
                Slug = post["Slug"].S,
                Title = post["Title"].S,
                Type = post["Type"].S,
                Content = CommonMark.CommonMarkConverter.Convert(content),
                HasMore = false
            };
        }

        public async Task<IEnumerable<PostSummary>> GetPosts(CancellationToken token)
        {
            var response = await amazonDynamoDb.QueryAsync(new QueryRequest
            {
                TableName = "BlogPosts",
                IndexName = "Type-Published-index",
                KeyConditionExpression = "#type = :published",
                ScanIndexForward = false,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":published", new AttributeValue { S =  "published" }}
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#type", "Type" }
                }
            }, token);

            var contentTasks = response.Items.Select(async post => {
                var slug = post["Slug"].S;
                var objectResponse = await amazonS3.GetObjectAsync("ae-posts", slug + ".md", token);
                using (var sr = new StreamReader(objectResponse.ResponseStream))
                {
                    var sb = new StringBuilder();
                    string line;
                    while (!sr.EndOfStream && (line = await sr.ReadLineAsync()) != "---")
                    {
                        sb.AppendLine(line);
                    }

                    return new PostSummary
                    {
                        Category = post["Category"].S,
                        Published = DateTime.Parse(post["Published"].S),
                        Slug = post["Slug"].S,
                        Title = post["Title"].S,
                        Type = post["Type"].S,
                        Content = CommonMark.CommonMarkConverter.Convert(sb.ToString()),
                        HasMore = !sr.EndOfStream
                    };
                }
            });

            return await Task.WhenAll(contentTasks);
        }
    }
}
