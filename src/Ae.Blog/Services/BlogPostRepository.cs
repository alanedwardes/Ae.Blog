using Ae.Blog.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ae.Blog.Services
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly IDictionary<string, object> cache = new ConcurrentDictionary<string, object>();
        private readonly IAmazonDynamoDB amazonDynamoDb;
        private readonly IConfiguration configuration;

        public BlogPostRepository(IAmazonDynamoDB amazonDynamoDb, IConfiguration configuration)
        {
            this.amazonDynamoDb = amazonDynamoDb;
            this.configuration = configuration;
        }

        public string TableName => configuration["POSTS_TABLE"];

        public async Task PutContent(Post post, CancellationToken token)
        {
            var attributeValues = new Dictionary<string, AttributeValue>
            {
                { "Title", new AttributeValue(post.Title) },
                { "Category", new AttributeValue(post.Category) },
                { "Type", new AttributeValue(post.Type.ToString().ToLowerInvariant()) },
                { "Content", new AttributeValue(post.Content) },
                { "Slug", new AttributeValue(post.Slug) },
                { "Published", new AttributeValue(post.Published.ToString("o")) }
            };

            if (post.Updated.HasValue)
            {
                attributeValues.Add("Updated", new AttributeValue(post.Updated.Value.ToString("o")));
            }

            await amazonDynamoDb.PutItemAsync(TableName, attributeValues, token);
            cache.Clear();
        }

        public async Task<Post> GetContent(string slug, CancellationToken token)
        {
            var cacheKey = $"{nameof(GetContent)}-{slug}";
            Post post;

            if (cache.TryGetValue(cacheKey, out var cachedValue))
            {
                post = (Post)cachedValue;
            }
            else
            {
                var item = await amazonDynamoDb.GetItemAsync(TableName, new Dictionary<string, AttributeValue>
                {
                    { "Slug", new AttributeValue(slug) }
                }, token);
                post = ItemToPost(item.Item);
                post.IsSingle = true;
                cache[cacheKey] = post;
            }
            return post;
        }

        public async Task<PostSummary[]> GetAllContentSummaries(CancellationToken token)
        {
            if (cache.TryGetValue(nameof(GetAllContentSummaries), out var cachedValue))
            {
                return (PostSummary[])cachedValue;
            }
            else
            {
                var postSummaries = (await amazonDynamoDb.ScanAsync(new ScanRequest
                {
                    TableName = TableName,
                    ProjectionExpression = "Slug,Category,Published,Title,#Type",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        { "#Type", "Type" }
                    },
                })).Items.Select(ItemToPost).OrderByDescending(x => x.Published).ToArray();
                cache[nameof(GetAllContentSummaries)] = postSummaries;
                return postSummaries;
            }
        }

        public async Task<Post[]> GetPublishedPosts(CancellationToken token)
        {
            if (cache.TryGetValue(nameof(GetPublishedPosts), out var cachedValue))
            {
                return (Post[])cachedValue;
            }
            else
            {
                var posts = await GetPostsInternal(new ScanRequest
                {
                    TableName = TableName,
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":published", new AttributeValue { S = "published" }},
                        {":featured", new AttributeValue { S = "featured" }}
                    },
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        { "#type", "Type" }
                    },
                    FilterExpression = "#type = :featured OR #type = :published"
                }, token);
                cache[nameof(GetPublishedPosts)] = posts;
                return posts;
            }
        }

        private async Task<Post[]> GetPostsInternal(ScanRequest query, CancellationToken token)
        {
            var response = await amazonDynamoDb.ScanAsync(query, token);

            return response.Items.Select(ItemToPost).OrderByDescending(x => x.Published).ToArray();
        }

        private Post ItemToPost(IDictionary<string, AttributeValue> item)
        {
            bool hasUpdated = item.ContainsKey("Updated") && item["Updated"].S != null;

            var post = new Post
            {
                Category = item["Category"].S,
                Published = DateTime.Parse(item["Published"].S),
                Updated = hasUpdated ? DateTime.Parse(item["Updated"].S) : new DateTime?(),
                Slug = item["Slug"].S,
                Title = item["Title"].S,
                Type = Enum.Parse<PostType>(item["Type"].S, true)
            };

            if (item.ContainsKey("Content"))
            {
                post.ContentRaw = item["Content"].S;
            }

            return post;
        }

        public async Task DeleteContent(string slug, CancellationToken token)
        {
            await amazonDynamoDb.DeleteItemAsync(new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Slug", new AttributeValue(slug) }
                }
            }, token);
            cache.Clear();
        }
    }
}
