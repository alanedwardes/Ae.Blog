using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace AeBlog.Data
{
    public class Post
    {
        public string Body { get; set; }
        [JsonProperty("is_published")]
        public bool IsPublished { get; set; }
        public DateTime Published { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
    }

    public class PostManager : IPostManager
    {
        private const string PostTable = "Aeblog.Post";
        private const string HashKey = "slug";
        private const string IsPublishedKey = "is_published";
        private const string IsPublishedIndex = "is_published-index";

        private readonly IDocumentStore documentStore;

        public PostManager(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public async Task<Post> GetPostBySlug(string slug, CancellationToken ctx = default(CancellationToken))
        {
            return (await documentStore.GetItems<Post>(PostTable, HashKey, slug, null, ctx)).SingleOrDefault();
        }

        public Task<IEnumerable<Post>> GetPublishedPosts(CancellationToken ctx = default(CancellationToken))
        {
            return documentStore.GetItems<Post>(PostTable, IsPublishedKey, 1, IsPublishedIndex, ctx);
        }
    }
}
