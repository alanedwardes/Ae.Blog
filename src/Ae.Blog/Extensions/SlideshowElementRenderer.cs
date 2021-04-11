using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Linq;

namespace Ae.Blog.Extensions
{
    public class SlideshowElementRenderer : HtmlObjectRenderer<SlideshowElement>
    {
        private readonly string CDN_URI = "http://s.edward.es";

        protected override void Write(HtmlRenderer renderer, SlideshowElement slideshow)
        {
            if (!slideshow.Images.Any())
            {
                return;
            }

            var firstImage = slideshow.Images.First();

            renderer.Write("<div class=\"slideshow\">");

            renderer.Write($"<a class=\"current slide\" href=\"{CDN_URI}/{firstImage}.webp\">");
            renderer.Write($"<img src=\"{CDN_URI}/{firstImage}-large.webp\"/>");
            renderer.Write($"</a>");

            renderer.Write("<div class=\"slides\">");
            foreach (var image in slideshow.Images)
            {
                renderer.Write($"<a class=\"slide preview\" onclick=\"showSlide(this);return false;\" " +
                               $"href=\"{CDN_URI}/{image}.webp\" " +
                               $"data-image-small=\"{CDN_URI}/{image}-small.webp\" " +
                               $"data-image-large=\"{CDN_URI}/{image}-large.webp\" " +
                               $"data-image-original=\"{CDN_URI}/{image}.webp\">");
                renderer.Write($"<img width=\"160\" src=\"{CDN_URI}/{image}-small.webp\"/>");
                renderer.Write("</a>");
            }
            renderer.Write("</div>");

            renderer.Write("</div>");
        }
    }
}
