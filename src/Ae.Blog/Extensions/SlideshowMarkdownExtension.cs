using Markdig;
using Markdig.Renderers;

namespace Ae.Blog.Extensions
{
    public class SlideshowMarkdownExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<SlideshowMarkdownParser>())
            {
                pipeline.BlockParsers.Insert(0, new SlideshowMarkdownParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<SlideshowElementRenderer>();
            }
        }
    }
}
