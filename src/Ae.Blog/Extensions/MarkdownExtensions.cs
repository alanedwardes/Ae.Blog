using Ae.Blog.Models;
using Markdig;
using Markdig.Extensions.MediaLinks;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ae.Blog.Extensions
{
    public static class MarkdownExtensions
    {
        private static readonly MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder()
                .UseEmphasisExtras()
                .UseAutoIdentifiers()
                .UsePipeTables()
                .UseMediaLinks(new MediaOptions{Width = "960",Height = "540"})
                .Build();

        public sealed class TextOnlyRendererer : IMarkdownExtension
        {
            public void Setup(MarkdownPipelineBuilder pipeline)
            {
            }

            public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
            {
                renderer.ObjectRenderers.Clear();
                renderer.ObjectRenderers.Add(new ParagraphRenderer());
                renderer.ObjectRenderers.Add(new ListRenderer());
                renderer.ObjectRenderers.Add(new HeadingRenderer());
                renderer.ObjectRenderers.Add(new HtmlBlockRenderer());
                renderer.ObjectRenderers.Add(new ParagraphRenderer());
                renderer.ObjectRenderers.Add(new QuoteBlockRenderer());
                renderer.ObjectRenderers.Add(new ThematicBreakRenderer());

                // Default inline renderers
                renderer.ObjectRenderers.Add(new AutolinkInlineRenderer());
                renderer.ObjectRenderers.Add(new DelimiterInlineRenderer());
                renderer.ObjectRenderers.Add(new EmphasisInlineRenderer());
                renderer.ObjectRenderers.Add(new LineBreakInlineRenderer());
                renderer.ObjectRenderers.Add(new HtmlInlineRenderer());
                renderer.ObjectRenderers.Add(new HtmlEntityInlineRenderer());
                renderer.ObjectRenderers.Add(new LinkInlineRenderer());
                renderer.ObjectRenderers.Add(new LiteralInlineRenderer());
            }
        }

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
            return Markdown.ToPlainText(post.ContentFirstLine, markdownPipeline).Trim();
        }

        public static void GetWordStatistics(this Post post, IDictionary<string, int> container)
        {
            var plainText = Markdown.ToPlainText(post.ContentAll, new MarkdownPipelineBuilder().Use<TextOnlyRendererer>().Build());

            plainText = Regex.Replace(plainText, @"http[^\s]+", "");

            var ignoredWords = new[]
            {
                "a", "the", "when", "at", "i", "to","and","of","is","in","for","you","it","this","on","that","with","be","can","as","are","use","your","from","not","will","an","but","which",
                "if","using","all","or", "any","my", "like","have","was","need","out","also","up","there","used","do","set","one","by","above","since","into",
                "only","so","some","same","see","other","then","would","approach","does","around","over","example",
                "things","them","very","no","run","has","more","few","feature","get","want","they","its", "because",
                "found", "here","following","new","should","however","these","check","had","once","where","first","could","large","allows","different","via","case","many",
                "post","support","may","work","must","problem","it's","than","how","were","such","address","good","each",
                "static", "app", "below", "process", "file", "features", "version", "hard", "time", "method", "project", "look", "system", "function", "server"
            };

            var protectedWords = new[]
            {
                "dns"
            };

            var punctuation = new[]
            {
                ' ', ',', '.', ':', ';', '!', '?', '\t', '[', ']', '(', ')', '<', '>', '='
            };

            var groups = plainText.Split(punctuation)
                                  .Select(x => x.ToLower().Trim())
                                  .Where(x => !ignoredWords.Contains(x))
                                  .Select(x => !protectedWords.Contains(x) && x.EndsWith("s") ? x[0..^1] : x)
                                  .Where(x => !int.TryParse(x, out var _))
                                  .Where(x => x.Length > 1)
                                  .GroupBy(x => x);

            foreach (var group in groups)
            {
                if (container.ContainsKey(group.Key))
                {
                    container[group.Key] += group.Count();
                }
                else
                {
                    container.Add(group.Key, group.Count());
                }
            }
        }
    }
}
