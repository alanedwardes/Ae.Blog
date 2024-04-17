using Ae.Blog.Models;
using Markdig;
using Markdig.Extensions.MediaLinks;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
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
                .Use<SlideshowMarkdownExtension>()
                .UseMediaLinks(new MediaOptions{Width = "960", Height = "540"})
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

        public static string GetMarkdown(this Post post, bool isSingle)
        {
            if (isSingle)
            {
                return Markdown.ToHtml(post.ContentAll, markdownPipeline);
            }

            return Markdown.ToHtml(post.ContentSummary, markdownPipeline);
        }

        public static string GetFirstLineText(this Post post)
        {
            var line = Markdown.ToPlainText(post.ContentFirstLine, markdownPipeline).Trim();

            var trimEndingCharacters = new[] { ':', ';' };

            if (trimEndingCharacters.Any(line.EndsWith))
            {
                line = line.TrimEnd(trimEndingCharacters) + '.';
            }

            return line;
        }

        private static readonly Regex IMAGE_URI_REGEX = new Regex("(poster|src)=\"(?<uri>.+?)\"");

        public static Uri GetFirstImage(this Post post)
        {
            foreach (Group group in IMAGE_URI_REGEX.Matches(post.GetMarkdown(true)).Select(x => x.Groups["uri"]))
            {
                if (new[] { ".png", ".jpg", ".jpeg", ".webp", ".gif" }.Any(group.Value.EndsWith) &&
                    Uri.TryCreate(group.Value, UriKind.Absolute, out Uri uri))
                {
                    return uri;
                }
            }

            return null;
        }

        public static string GetPlainText(this Post post)
        {
            var plainText = Markdown.ToPlainText(post.ContentAll, new MarkdownPipelineBuilder().Use<TextOnlyRendererer>().Build());

            plainText = Regex.Replace(plainText, @"http[^\s]+", "");

            return plainText;
        }

        private static readonly char[] permittedPunctuation = new[] { '\'', '+', '@', '#', '♯', '-' };

        public static string[] SplitTextIntoWords(string text)
        {
            var punctuation = Enumerable.Range(char.MinValue, char.MaxValue)
                .Select(x => (char)x)
                .Where(x => !char.IsLetterOrDigit(x) && !permittedPunctuation.Contains(x))
                .Distinct()
                .ToArray();

            return text.Split(punctuation);
        }

        private static readonly Lazy<string[]> _redundantWords = new(() => File.ReadAllLines("Resources/redundant_words.txt"));
        private static readonly Lazy<Dictionary<string, string>> _wordRemaps = new(() => File.ReadAllLines("Resources/word_remaps.csv").Select(x => x.Split(',')).ToDictionary(x => x[0], x => x[1]));

        public static IDictionary<string, int> GetWordStatistics(IEnumerable<string> words)
        {
            var container = new Dictionary<string, int>();

            var groups = words.Select(x => x.ToLower().Trim())
                        .Select(x => x.EndsWith("'s") ? x[..^2] : x)
                        .Select(x => _wordRemaps.Value.ContainsKey(x) ? _wordRemaps.Value[x] : x)
                        .Where(x => !_redundantWords.Value.Contains(x))
                        .Where(x => !Guid.TryParse(x, out var id))
                        .Where(x => !int.TryParse(x, out var _))
                        .Where(x => x.Count(permittedPunctuation.Contains) < 2)
                        .Where(x => x.Length > 1 && x.Length < 20)
                        .GroupBy(x => x);

            foreach (var group in groups)
            {
                container.Add(group.Key, group.Count());
            }

            return container;
        }
    }
}
