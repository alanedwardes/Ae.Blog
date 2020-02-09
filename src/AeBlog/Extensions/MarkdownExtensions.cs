using AeBlog.Models;
using CommonMark;
using System;

namespace AeBlog.Extensions
{
    public static class MarkdownExtensions
    {
        private static Lazy<CommonMarkSettings> converterSettings = new Lazy<CommonMarkSettings>(() =>
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.AdditionalFeatures |= CommonMarkAdditionalFeatures.StrikethroughTilde;
            return settings;
        });

        public static string GetMarkdown(this Post post)
        {
            if (post.IsSingle)
            {
                return CommonMarkConverter.Convert(post.ContentAll, converterSettings.Value);
            }

            return CommonMarkConverter.Convert(post.ContentSummary, converterSettings.Value);
        }
    }
}
