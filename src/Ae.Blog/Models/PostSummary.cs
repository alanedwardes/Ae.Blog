using Ae.Blog.Models.Admin;
using System;

namespace Ae.Blog.Models
{
    public class PostSummary
    {
        public string Slug { get; set; }
        public string Category { get; set; }
        public DateTime Published { get; set; }
        public DateTime? Updated { get; set; }
        public string Title { get; set; }
        public PostType Type { get; set; }

        public string Url => Type == PostType.Page ? $"/{Slug}/" : $"/blog/posts/{Slug}/";
        public string CategoryUrl => $"/blog/categories/{Category}/";
    }
}
