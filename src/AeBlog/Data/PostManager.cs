using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AeBlog.Caching;

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
        private readonly ICacheProvider cacheProvider;

        public PostManager(ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        private async Task<IEnumerable<Post>> GetPosts(CancellationToken ctx)
        {
            return await cacheProvider.Get<IList<Post>>("posts", ctx) ?? Enumerable.Empty<Post>();
        }

        public async Task<Post> GetPostBySlug(string slug, CancellationToken ctx)
        {
            return (await GetPosts(ctx)).Where(p => p.Slug == slug && p.IsPublished).SingleOrDefault();
        }

        public async Task<IEnumerable<Post>> GetPublishedPosts(CancellationToken ctx)
        {
            return (await GetPosts(ctx)).Where(p => p.IsPublished);
        }
    }
}
