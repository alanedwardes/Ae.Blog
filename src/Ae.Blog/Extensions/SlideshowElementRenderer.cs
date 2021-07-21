using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Linq;

namespace Ae.Blog.Extensions
{
    public class SlideshowElementRenderer : HtmlObjectRenderer<SlideshowElement>
    {
        protected override void Write(HtmlRenderer renderer, SlideshowElement slideshow)
        {
            if (!slideshow.Images.Any())
            {
                return;
            }

            var firstImage = slideshow.Images.First();

            renderer.Write("<div class=\"slideshow\">");

            renderer.Write($"<a class=\"current slide\" target=\"_new\" href=\"{Constants.CDN_ROOT}{firstImage}.webp\">");
            renderer.Write($"<img onload=\"this.parentElement.classList.remove('loading')\" src=\"{Constants.CDN_ROOT}{firstImage}-large.webp\"/>");
            renderer.Write($"</a>");

            renderer.Write("<div class=\"slides\">");
            foreach (var image in slideshow.Images)
            {
                renderer.Write($"<a class=\"slide preview\" target=\"_new\" onclick=\"showSlide(this);return false;\" " +
                               $"href=\"{Constants.CDN_ROOT}{image}.webp\" " +
                               $"data-image-small=\"{Constants.CDN_ROOT}{image}-small.webp\" " +
                               $"data-image-large=\"{Constants.CDN_ROOT}{image}-large.webp\" " +
                               $"data-image-original=\"{Constants.CDN_ROOT}{image}.webp\">");
                renderer.Write($"<img width=\"160\" src=\"{Constants.CDN_ROOT}{image}-small.webp\"/>");
                renderer.Write("</a>");
            }
            renderer.Write("</div>");

            renderer.Write("</div>");
        }
    }
}
