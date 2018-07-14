using CommonMark;
using CommonMark.Formatters;
using CommonMark.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;

namespace AeBlog.Services
{

    public class AmpHtmlFormatter : HtmlFormatter
    {
        private readonly IImageRepository imageRepository;

        public AmpHtmlFormatter(IServiceProvider provider, TextWriter target, CommonMarkSettings settings) : base(target, settings)
        {
            imageRepository = provider.GetRequiredService<IImageRepository>();
        }

        protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            if (inline.Tag == InlineTag.Image && !RenderPlainTextInlines.Peek())
            {
                ignoreChildNodes = false;

                if (isOpening)
                {
                    Write("<amp-img layout=\"responsive\" src=\"");
                    var uriResolver = Settings.UriResolver;
                    if (uriResolver != null)
                    {
                        WriteEncodedUrl(uriResolver(inline.TargetUrl));
                    }
                    else
                    {
                        WriteEncodedUrl(inline.TargetUrl);
                    }

                    Write("\"");

                    try
                    {
                        var dimensions = imageRepository.GetImageDimensions(inline.TargetUrl, CancellationToken.None).GetAwaiter().GetResult();
                        Write($" width=\"{dimensions.Item1}\" height=\"{dimensions.Item2}\"");
                    }
                    catch (Exception)
                    {
                    }

                    Write(" alt=\"");

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
                }
            }
            else
            {
                base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
            }
        }
    }
}
