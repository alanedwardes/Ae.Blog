﻿using System;

namespace Ae.Blog.Models
{
    public class Post : PostSummary
    {
        private const string MoreMarker = "---";

        public string Content { get; set; }
        public bool HasSummary => Content.Contains(MoreMarker);
        public string ContentAll => Content.Replace(MoreMarker, string.Empty);
        public string ContentSummary => Content.Split(MoreMarker)[0];
        public string ContentFirstLine => Content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
        public bool IsSingle { get; set; }
    }
}
