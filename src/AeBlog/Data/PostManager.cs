using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public class Post
    {
        public string body { get; set; }
        public bool is_published { get; set; }
        public DateTime published { get; set; }
        public string slug { get; set; }
        public string title { get; set; }
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
            return documentStore.GetItemAsync<Post>(slug, PostTable);
        }
    }
}
