using Ae.Blog.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ae.Blog.Services
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ILogger<BlogPostRepository> logger;
        private readonly IAmazonDynamoDB amazonDynamoDb;
        private readonly IConfiguration configuration;
        private Task<Post[]> allPosts;

        public BlogPostRepository(ILogger<BlogPostRepository> logger, IAmazonDynamoDB amazonDynamoDb, IConfiguration configuration)
        {
            this.logger = logger;
            this.amazonDynamoDb = amazonDynamoDb;
            this.configuration = configuration;
            ReloadPosts();
        }

        private void ReloadPosts()
        {
            allPosts = GetAllPosts(CancellationToken.None);
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
            ReloadPosts();
        }

        public async Task<Post> GetContent(string slug, CancellationToken token)
        {
            return (await allPosts).Single(x => x.Slug == slug);
        }

        public async Task<PostSummary[]> GetAllContentSummaries(CancellationToken token)
        {
            return await allPosts;
        }

        public async Task<Post[]> GetPublishedPosts(CancellationToken token)
        {
            return (await allPosts).Where(x => x.Type == PostType.Featured || x.Type == PostType.Published || x.Type == PostType.Demoted).ToArray();
        }

        private async Task<Post[]> GetAllPosts(CancellationToken token)
        {
            logger.LogInformation("Begin pulling posts from DynamoDB");
            var response = await amazonDynamoDb.ScanAsync(new ScanRequest
            {
                TableName = TableName
            }, token);
            logger.LogInformation("Retrieved {PostCount} posts from DynamoDB", response.Items.Count);

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
                post.Content = item["Content"].S
                    .Replace("https://s.edward.es/", Constants.CDN_ROOT.ToString())
                    .Replace("$CDN_DOMAIN$", Constants.CDN_ROOT.ToString())
                    .Replace("alan.gdn", Constants.CDN_ROOT.Host)
                    .Replace("$STATIC_ASSET_PREFIX$", configuration["STATIC_ASSET_PREFIX"]);
                post.PreCompute();
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
            ReloadPosts();
        }
    }
}
