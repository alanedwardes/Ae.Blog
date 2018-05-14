﻿using AeBlog.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Services
{

    public class BlogPostRetriever : IBlogPostRetriever
    {
        private readonly IAmazonDynamoDB amazonDynamoDb;
        private const string MoreMarker = "---";

        public BlogPostRetriever(IAmazonDynamoDB amazonDynamoDb)
        {
            this.amazonDynamoDb = amazonDynamoDb;
        }

        public async Task PutPost(Post post, CancellationToken token)
        {
            LambdaLogger.Log(post.Title);
            LambdaLogger.Log(post.Category);
            LambdaLogger.Log(post.Type);
            LambdaLogger.Log(post.Content.Markdown);
            LambdaLogger.Log(post.Slug);
            LambdaLogger.Log(post.Published.ToString("o"));

            await amazonDynamoDb.PutItemAsync("BlogPosts", new Dictionary<string, AttributeValue>
            {
                { "Title", new AttributeValue(post.Title) },
                { "Category", new AttributeValue(post.Category) },
                { "Type", new AttributeValue(post.Type) },
                { "Content", new AttributeValue(post.Content.Markdown) },
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

            return response.Items.Select(ItemToPost).ToArray();
        }

        private Post ItemToPost(IDictionary<string, AttributeValue> item)
        {
            return new Post
            {
                Category = item["Category"].S,
                Published = DateTime.Parse(item["Published"].S),
                Slug = item["Slug"].S,
                Title = item["Title"].S,
                Type = item["Type"].S,
                Content = new PostContent { Markdown = item["Content"].S }
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
