using System;

namespace AeBlog.Models
{
    public class PostSummary
    {
        public string Slug { get; set; }
        public string Category { get; set; }
        public DateTime Published { get; set; }
        public DateTime? Updated { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public string Url => $"/blog/posts/{Slug}/";
        public string CategoryUrl => $"/blog/category/{Category}/";
    }
}
