using AeBlog.Models;
using Markdig;

namespace AeBlog.Extensions
{
    public static class MarkdownExtensions
    {
        private static readonly MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder()
                .UseEmphasisExtras()
                .UseAutoIdentifiers()
                .Build();

        public static string GetMarkdown(this Post post)
        {
            if (post.IsSingle)
            {
                return Markdown.ToHtml(post.ContentAll, markdownPipeline);
            }

            return Markdown.ToHtml(post.ContentSummary, markdownPipeline);
        }

        public static string GetFirstLineText(this Post post)
        {
            return Markdown.ToHtml(post.ContentFirstLine, markdownPipeline).Trim();
        }
    }
}
