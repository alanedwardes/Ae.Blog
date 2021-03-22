using System.Collections.Generic;

namespace Ae.Blog.Models
{
    public class BlogModel
    {
        public string FilterValue { get; set; }
        public string FilterType { get; set; }
        public IList<PostSummary> Archive { get; set; }
        public IList<Post> Posts { get; set; }
        public Post Single { get; set; }
    }
}
