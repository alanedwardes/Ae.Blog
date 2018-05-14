﻿using System.ComponentModel.DataAnnotations;

namespace AeBlog.Models.Admin
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
        [Required]
        public bool IsPublished { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
