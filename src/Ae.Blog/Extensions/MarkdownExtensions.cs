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

            var ignoredWords = new HashSet<string>
            {
                "a", "the", "when", "at", "i", "to","and","of","is","in","for","you","it","this","on","that","with","be",
                "can","as","are","use","your","from","not","will","an","but","which","if","using","all","or", "any","my",
                "like","have","was","need","out","also","up","there","used","do","set","one","by","above","since","into",
                "only","so","some","same","see","other","then","would","approach","does","around","over","example",
                "things","them","very","no","run","has","more","few","feature","get","want","they","its", "because",
                "found", "here","following","new","should","however","these","check","had","once","where","first","could",
                "large","allows","different","via","case","many","post","support","may","work","must","problem","it's",
                "than","how","were","such","address","good","each","static", "app", "below", "process", "features",
                "version", "hard", "time", "method", "project", "look", "system", "function", "server", "we", "identical",
                "content", "start", "here's", "benefit", "real", "use", "uses", "person", "benefit", "you're", "just", "test",
                "especially", "updating", "look", "great", "wanted", "ability", "per", "include", "point", "return", "across",
                "various", "shouldn't", "called", "last", "lot", "limited", "moving", "place", "object", "car", "little", "allow",
                "even", "relies", "didn't", "receive", "still", "felt", "their", "forums", "applied", "piece", "caveat", "pieces",
                "decision", "mean", "either", "order", "fully", "recently", "introduced", "move", "toward", "needed", "looked",
                "completely", "created", "second", "containing", "three", "hanging", "showing", "represent", "much", "believable",
                "fashion", "result", "while", "lumpy", "isn't", "noticable", "look", "better", "without", "embarked", "upon", "wish",
                "won't", "review", "involved", "scale", "applies", "invaluable", "instance", "gather", "believe", "wysiwyg", "backed",
                "closest", "technology", "compare", "winform", "codebehind", "borrow", "concept", "wpf", "lack", "navigation", "up/down",
                "pressing", "click", "doesn't", "list", "sure", "start", "off", "touch", "most", "bit", "figuring", "robust", "implementation",
                "obsolete", "go", "incredible", "in-world", "manually", "design", "simple", "shape", "rich", "manipulating", "grew", "began",
                "grind", "halt", "setting", "defer", "processing", "shifted", "delay", "build", "became", "pretty", "odd", "primitive", "blocking",
                "package", "beta", "editing", "wrap-up", "hear", "advice", "title", "other", "myself", "anyone", "forget", "lesson", "learned",
                "looking", "future", "take", "perhaps", "depend", "highly", "instead", "available", "clearly", "portion", "fall", "wayside", "pick",
                "nor", "usage", "identify", "actively", "developed", "that's", "we'll", "expected", "returning", "whatever", "fit", "status",
                "testing", "loading", "running", "spanning", "recursively", "gather", "archive", "update", "size", "negated", "pushing", "stringent",
                "this", "entire", "now", "trip", "between", "those", "reality", "really", "evident", "anything", "matching", "entirely", "deployed",
                "gathering", "publishing", "invoked", "finish", "way", "whole", "com", "gathered", "pushed", "push", "button", "there's", "current",
                "gathers", "automatically", "try", "possible", "else", "decided", "proof", "meant", "baking", "describing", "our", "full", "overview",
                "seem", "they're", "two", "separate", "times", "meaning", "begin", "purposes", "accept", "final", "follow", "simpler", "safer", "onwards",
                 "effect", "results", "careful", "high", "quality", "name", "suggests", "suitable", "tell", "spot", "offer", "don't", "writing", "who",
                "you'll", "hidden", "usually", "small", "card", "comes", "least", "later", "depending", "solutions", "masking", "shuffling", "retrieve",
                "involves", "pointed", "wrote", "phrases", "names", "though", "got", "appears", "page", "match", "down", "shown", "finally", "kinds",
                "specified", "weekends", "weekend", "absolute", "opposite", "chosen", "keeping", "pulling", "newer", "type", "recent", "i've", "vast",
                "majority", "preferred", "suspect", "supports", "support", "though", "me", "why", "third", "call", "done", "particularly", "pay", "what's",
                "typically", "having", "makes", "easier", "easily", "until", "passes", "again", "commented", "probably", "means", "removed", "yet", "path", "feels",
                "every", "bind", "stop", "disable", "disabled", "create", "excellent", "ones", "too", "included", "permitting", "permit", "typical", "described",
                "seeing", "regular", "certain", "accordingly", "adjust", "somewhere", "big", "prior", "going", "obtain", "further", "wraps", "functionality",
                "act", "both", "mostly", "whilst", "providing", "less", "takes", "optimal", "right", "depends", "preferably", "itself", "therefore", "attempted",
                "doing", "eventually", "able", "find", "equivalent", "exposed", "clear", "clears", "trivial", "achieve", "rather", "doing", "assume", "haven't", "after",
                "true", "false", "been", "made", "working", "home", "runs", "run", "modify", "today", "exactly", "conceived", "wasn't", "main", "interested", "works",
                "changed", "change", "verify", "bringing", "covers", "normally", "ahead", "giving", "context", "submitting", "nature", "avoid", "spent", "affected", "reasons",
                "tested", "opportunity", "decisions", "hitting", "hold", "solve", "solves", "common", "alone", "mentioned", "based", "people", "added", "never", "what",
                "obviously", "made", "am", "original", "deciding", "what", "preserve", "own", "solution", "likely", "trying", "sell", "bad", "sell", "par", "course", "unfortunately",
                "increasingly", "releasing", "closer", "glance", "red", "dead", "redemption", "widely", "requires", "looks", "ensure", "pass", "sent", "loaded", "add", "needs", "might",
                "said", "wrong", "offers", "pointing", "simplify", "us", "grounded", "bypassing", "adding", "fill", "under", "users", "user's", "user", "points", "write", "explain", "consists",
                "open", "references", "reference", "being", "always", "break", "make", "applicable", "img", "width", "height", "src", "mimicked", "prefer", "lives", "benefits",
                "provides", "number", "numbers", "share", "safe", "predictable", "includes", "founders", "towards", "starts", "showcasing", "technique", "received", "information", "contact",
                "visually", "difficult", "distinguish", "increase", "noticed", "control", "miscellaneous", "service", "services", "opt", "handling", "killing"
            };

            var remappedWords = new Dictionary<string, string>
            {
                { "queries", "query" },
                { "binaries", "binary" },
                { "site", "website" },
                { "sites", "website" },
                { "websites", "website" },
                { "website's", "website" },
                { "files", "file" },
                { "requests", "request" },
                { "levels", "level" },
                { "released", "release" },
                { "developers", "developer" },
                { "platforms", "platform" },
                { "assets", "asset" },
                { "blueprints", "blueprint" },
                { "players", "player" },
                { "plugins", "plugin" },
                { "bugs", "bug" },
                { "domains", "domain" },
                { "games", "game" },
                { "flags", "flag" },
                { "shadows", "shadow" },
                { "characters", "character" },
                { "lights", "light" },
                { "lighting", "light" },
                { "threads", "thread" },
                { "builds", "build" }
            };

            var punctuation = new[]
            {
                ' ', ',', '.', ':', ';', '!', '?', '\t', '[', ']', '(', ')', '<', '>', '='
            };

            var groups = plainText.Split(punctuation)
                                  .Select(x => x.ToLower().Trim())
                                  .Select(x => remappedWords.ContainsKey(x) ? remappedWords[x] : x)
                                  .Where(x => !ignoredWords.Contains(x))
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
