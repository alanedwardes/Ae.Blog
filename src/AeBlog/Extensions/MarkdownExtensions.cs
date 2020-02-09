using AeBlog.Models;
using CommonMark;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.IO;

namespace AeBlog.Extensions
{
    public static class MarkdownExtensions
    {
        private static Lazy<CommonMarkSettings> htmlConverterSettings = new Lazy<CommonMarkSettings>(() =>
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.AdditionalFeatures |= CommonMarkAdditionalFeatures.StrikethroughTilde;
            return settings;
        });

        private static Lazy<CommonMarkSettings> plainTextConverterSettings = new Lazy<CommonMarkSettings>(() =>
        {
            var settings = htmlConverterSettings.Value.Clone();
            settings.OutputDelegate = (b, w, s) => PrintBlocks(w, b, s);
            return settings;
        });

        public static string GetMarkdown(this Post post)
        {
            if (post.IsSingle)
            {
                return CommonMarkConverter.Convert(post.ContentAll, htmlConverterSettings.Value);
            }

            return CommonMarkConverter.Convert(post.ContentSummary, htmlConverterSettings.Value);
        }

        public static string GetFirstLineText(this Post post)
        {
            return CommonMarkConverter.Convert(post.ContentFirstLine, plainTextConverterSettings.Value).Trim();
        }

        public static void PrintBlocks(TextWriter writer, Block block, CommonMarkSettings settings)
        {
            var stack = new Stack<Block>();
            var inlineStack = new Stack<Inline>();

            while (block != null)
            {
                if (block.InlineContent != null)
                {
                    PrintInlines(writer, block.InlineContent, inlineStack);
                }

                if (block.FirstChild != null)
                {
                    if (block.NextSibling != null)
                    {
                        stack.Push(block.NextSibling);
                    }

                    block = block.FirstChild;
                }
                else if (block.NextSibling != null)
                {
                    block = block.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    block = stack.Pop();
                }
                else
                {
                    block = null;
                }

                writer.WriteLine();
            }
        }

        private static void PrintInlines(TextWriter writer, Inline inline, Stack<Inline> stack)
        {
            while (inline != null)
            {
                switch (inline.Tag)
                {
                    case InlineTag.String:
                        writer.Write(inline.LiteralContent);
                        break;

                    case InlineTag.Code:
                        writer.Write(inline.LiteralContent);
                        break;

                    case InlineTag.RawHtml:
                        writer.Write(inline.LiteralContent);
                        break;

                    case InlineTag.Link:
                        writer.Write(inline.LiteralContent);
                        break;

                    case InlineTag.Image:
                        writer.Write(inline.LiteralContent);
                        break;
                }

                if (inline.FirstChild != null)
                {
                    if (inline.NextSibling != null)
                    {
                        stack.Push(inline.NextSibling);
                    }

                    inline = inline.FirstChild;
                }
                else if (inline.NextSibling != null)
                {
                    inline = inline.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    inline = stack.Pop();
                }
                else
                {
                    inline = null;
                }
            }
        }
    }
}
