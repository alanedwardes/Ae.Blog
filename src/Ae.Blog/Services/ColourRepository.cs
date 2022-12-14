using SixLabors.ImageSharp;
using System;

namespace Ae.Blog.Services
{
    public interface IColourRepository
    {
        Color GetHighlightColour();
    }

    internal sealed class ColourRepository : IColourRepository
    {
        private readonly Random random = new Random();

        public Color GetHighlightColour()
        {
            var colours = new []{ Color.LightSeaGreen, Color.Crimson, Color.LightSlateGray };

            return colours[random.Next(colours.Length)];
        }
    }
}
