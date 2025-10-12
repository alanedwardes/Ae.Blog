using Amazon.CloudFront.Model;

namespace Ae.Blog.Models.Admin
{
    public class AdminModel
    {
        public Post Post { get; set; }
        public PostSummary[] Posts { get; set; }
    }
}
