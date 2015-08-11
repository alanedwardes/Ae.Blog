using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AeBlog
{
    public static class UriExtensions
    {
        public static Uri AddQueryParameters(this Uri uri, IDictionary<string, object> parameters)
        {
            if (!parameters.Any())
            {
                return uri;
            }

            var builder = new StringBuilder();
            var first = true;
            foreach (var parameter in parameters)
            {
                builder.Append(first ? '?' : '&');
                builder.Append($"{parameter.Key}={parameter.Value.ToString()}");
                first = false;
            }

            return new Uri(uri, builder.ToString());
        }
    }
}
