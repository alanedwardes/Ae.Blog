using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        private readonly IDocumentStore documentStore;

        public PostManager(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public Task<Post> GetPostBySlug(string slug, CancellationToken ctx = default(CancellationToken))
        {
            return documentStore.GetItem<Post>(slug, PostTable, ctx);
        }

        public Task<IEnumerable<Post>> GetPosts(CancellationToken ctx = default(CancellationToken))
        {
            return documentStore.GetItems<Post>(PostTable, ctx);
        }
    }
}
