using Markdig.Parsers;
using System;
using System.Linq;

namespace Ae.Blog.Extensions
{
    public class SlideshowMarkdownParser : BlockParser
    {
        public SlideshowMarkdownParser()
        {
            OpeningCharacters = new[] {'£'};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // We expect no indentation for a slideshow block.
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            // Ignore £
            processor.NextChar();

            var images = processor.Line
                .ToString()
                .Trim()
                .Split(" ")
                .Where(x => Guid.TryParse(x, out _))
                .Select(Guid.Parse)
                .ToArray();

            var figure = new SlideshowElement(this, images);
            processor.NewBlocks.Push(figure);

            // Discard the current line as it is already parsed
            return BlockState.ContinueDiscard;
        }
    }
}
