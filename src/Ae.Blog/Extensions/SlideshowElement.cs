using Markdig.Parsers;
using Markdig.Syntax;
using System;

namespace Ae.Blog.Extensions
{
    public sealed class SlideshowElement : Block
    {
        public SlideshowElement(BlockParser parser, Guid[] images) : base(parser)
        {
            Images = images;
        }

        public Guid[] Images { get; }
    }
}
