using Ae.Blog.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ae.Blog.Models
{
    public class Post : PostSummary
    {
        private const string MoreMarker = "---";
        public string Content { get; set; }

        [JsonIgnore]
        public bool HasSummary => Content.Contains(MoreMarker);

        [JsonIgnore]
        public string ContentAll => Content.Replace(MoreMarker, string.Empty);

        [JsonIgnore]
        public string ContentSummary => Content.Split(MoreMarker)[0];

        [JsonIgnore]
        public string ContentFirstLine => Content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries)[0];

        public void PreCompute()
        {
            ContentPlainText = this.GetPlainText();
            ContentWords = MarkdownExtensions.SplitTextIntoWords(Title + " " + ContentPlainText);
            ContentWordStatistics = MarkdownExtensions.GetWordStatistics(ContentWords);
        }

        [JsonIgnore]
        public string ContentPlainText { get; set; }

        [JsonIgnore]
        public IDictionary<string, int> ContentWordStatistics { get; set; }

        [JsonIgnore]
        public IList<string> ContentWords { get; set; }

        [JsonIgnore]
        public string ContentRaw { get; internal set; }
    }
}
