using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AeBlog.Extensions
{
    public static class DynamoExtensions
    {
        public static IEnumerable<TItem> Deserialize<TItem>(this IEnumerable<Document> documents)
        {
            foreach (var document in documents)
            {
                yield return JsonConvert.DeserializeObject<TItem>(document.ToJson());
            }
        }
    }
}
