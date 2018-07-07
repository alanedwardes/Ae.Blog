using AeBlog.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Services
{

    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly IAmazonDynamoDB amazonDynamoDb;
        private const string MoreMarker = "---";

        public BlogPostRepository(IAmazonDynamoDB amazonDynamoDb)
        {
            this.amazonDynamoDb = amazonDynamoDb;
        }

        public async Task PutPost(Post post, CancellationToken token)
        {
            await amazonDynamoDb.PutItemAsync("BlogPosts", new Dictionary<string, AttributeValue>
            {
                { "Title", new AttributeValue(post.Title) },
                { "Category", new AttributeValue(post.Category) },
                { "Type", new AttributeValue(post.Type) },
                { "Content", new AttributeValue(post.Content) },
                { "Slug", new AttributeValue(post.Slug) },
                { "Published", new AttributeValue(post.Published.ToString("o")) }
            }, token);
        }

        public async Task<Post> GetPost(string slug, CancellationToken token)
        {
            var item = await amazonDynamoDb.GetItemAsync("BlogPosts", new Dictionary<string, AttributeValue>
            {
                { "Slug", new AttributeValue(slug) }
            }, token);

            var post = ItemToPost(item.Item);
            post.IsSingle = true;
            return post;
        }

        public async Task<PostSummary[]> GetAllPostSummaries(CancellationToken token)
        {
            return (await amazonDynamoDb.ScanAsync(new ScanRequest
            {
                TableName = "BlogPosts",
                ProjectionExpression = "Slug,Category,Published,Title,#Type",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#Type", "Type" }
                }
            })).Items.Select(ItemToPost).OrderByDescending(x => x.Published).ToArray();
        }

        public async Task<Post[]> GetPublishedPosts(CancellationToken token)
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

            return response.Items.Select(ItemToPost).ToArray();
        }

        private Post ItemToPost(IDictionary<string, AttributeValue> item)
        {
            var post = new Post
            {
                Category = item["Category"].S,
                Published = DateTime.Parse(item["Published"].S),
                Slug = item["Slug"].S,
                Title = item["Title"].S,
                Type = item["Type"].S
            };

            if (item.ContainsKey("Content"))
            {
                post.Content = item["Content"].S;
            }

            return post;
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

        public async Task<PostSummary[]> GetPublishedPostSummaries(CancellationToken token)
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

        public async Task DeletePost(string slug, CancellationToken token)
        {
            await amazonDynamoDb.DeleteItemAsync(new DeleteItemRequest
            {
                TableName = "BlogPosts",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Slug", new AttributeValue(slug) }
                }
            }, token);
        }
    }
}
