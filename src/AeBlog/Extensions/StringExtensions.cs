using System.Text.RegularExpressions;

namespace AeBlog.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex Slug = new Regex("[^a-z0-9 ]");

        public static string ToSlug(this string input)
        {
            return Slug.Replace(input.ToLowerInvariant(), string.Empty).Replace(" ", "-");
        }
    }
}
