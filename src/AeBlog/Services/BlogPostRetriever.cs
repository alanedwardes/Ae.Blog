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
        private const string MoreMarker = "---";

        public BlogPostRetriever(IAmazonDynamoDB amazonDynamoDb, IAmazonS3 amazonS3)
        {
            this.amazonDynamoDb = amazonDynamoDb;
            this.amazonS3 = amazonS3;
        }

        public async Task<Tuple<bool, string>> GetPostContent(string slug, bool isFullPost, CancellationToken token)
        {
            var response = await amazonS3.GetObjectStreamAsync("ae-posts", slug + ".md", null, token);
            using (var sr = new StreamReader(response))
            {
                var sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    string line = await sr.ReadLineAsync();
                    if (line != MoreMarker)
                    {
                        sb.AppendLine(line);
                    }
                    else if (!isFullPost)
                    {
                        break;
                    }
                }

                var content = CommonMark.CommonMarkConverter.Convert(sb.ToString());

                return Tuple.Create(!sr.EndOfStream, content);
            }
        }

        public async Task<Post> GetPost(string slug, CancellationToken token)
        {
            var metaTask = amazonDynamoDb.GetItemAsync("BlogPosts", new Dictionary<string, AttributeValue>
            {
                { "Slug", new AttributeValue(slug) }
            }, token);

            var contentTask = GetPostContent(slug, true, token);

            var post = ItemToPost((await metaTask).Item);

            var content = await contentTask;
            post.Content = content.Item2;
            post.IsSingle = true;
            return post;
        }

        public async Task<Post[]> GetPosts(CancellationToken token)
        {
            return await GetPostsInternal(new QueryRequest
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
        }

        private async Task<Post[]> GetPostsInternal(QueryRequest query, CancellationToken token)
        {
            var response = await amazonDynamoDb.QueryAsync(query, token);

            var contentTasks = response.Items.Select(async item => {
                var post = ItemToPost(item);
                var content = await GetPostContent(post.Slug, false, token);

                post.HasMore = content.Item1;
                post.Content = content.Item2;

                return post;
            });

            return await Task.WhenAll(contentTasks);
        }

        private Post ItemToPost(IDictionary<string, AttributeValue> item)
        {
            return new Post
            {
                Category = item["Category"].S,
                Published = DateTime.Parse(item["Published"].S),
                Slug = item["Slug"].S,
                Title = item["Title"].S,
                Type = item["Type"].S
            };
        }

        public async Task<Post[]> GetPostsForCategory(string category, CancellationToken token)
        {
            return await GetPostsInternal(new QueryRequest
            {
                TableName = "BlogPosts",
                IndexName = "Category-Published-index",
                KeyConditionExpression = "#category = :category",
                ScanIndexForward = false,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":category", new AttributeValue { S =  category }}
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#category", "Category" }
                }
            }, token);
        }

        public async Task<PostSummary[]> GetPostSummaries(CancellationToken token)
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

            return response.Items.Select(ItemToPost).ToArray();
        }
    }
}
