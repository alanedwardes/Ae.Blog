using CommonMark;
using CommonMark.Formatters;
using CommonMark.Syntax;
using System.IO;

namespace AeBlog.Services
{
    public class AmpHtmlFormatter : HtmlFormatter
    {
        public AmpHtmlFormatter(TextWriter target, CommonMarkSettings settings) : base(target, settings)
        {
        }

        protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            if (inline.Tag == InlineTag.Image && !RenderPlainTextInlines.Peek())
            {
                ignoreChildNodes = false;

                if (isOpening)
                {
                    Write("<div class=\"image-container\">");
                    Write("<amp-img class=\"contain\" layout=\"fill\" src=\"");
                    var uriResolver = Settings.UriResolver;
                    if (uriResolver != null)
                    {
                        WriteEncodedUrl(uriResolver(inline.TargetUrl));
                    }
                    else
                    {
                        WriteEncodedUrl(inline.TargetUrl);
                    }

                    Write("\" alt=\"");

                    if (!isClosing)
                    {
                        RenderPlainTextInlines.Push(true);
                    }
                }

                if (isClosing)
                {
                    Write('\"');
                    if (inline.LiteralContent.Length > 0)
                    {
                        Write(" title=\"");
                        WriteEncodedHtml(inline.LiteralContent);
                        Write('\"');
                    }

                    if (Settings.TrackSourcePosition)
                    {
                        WritePositionAttribute(inline);
                    }
                    Write("></amp-img>");
                    Write("</div>");
                }
            }
            else
            {
                base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
            }
        }
    }
}
