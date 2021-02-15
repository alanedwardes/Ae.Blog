using System.ComponentModel.DataAnnotations;

namespace Ae.Blog.Models.Admin
{
    public class EditPostModel
    {
        [Required]
        [MaxLength(256)]
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [Required]
        [MaxLength(256)]
        [DataType(DataType.Text)]
        public string Category { get; set; }
        [MaxLength(256)]
        [DataType(DataType.Text)]
        public string Slug { get; set; }
        [Required]
        public bool IsPublished { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
