using AeBlog.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Clients
{
    public enum JsonAlbumImageSize
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public class JsonAlbumArtist
    {
        public string Name { get; set; }
        public Guid? mbid { get; set; }
        public Uri Url { get; set; }
    }

    public class Album
    {
        public string Name { get; set; }
        public int Playcount { get; set; }
        public Uri Url { get; set; }
        public JsonAlbumArtist Artist { get; set; }
        public Uri Thumbnail { get; set; }
        public int Rank { get; set; }

        public override string ToString() => Artist == null ? Name : Name + " by " + Artist.Name;
    }

    public class JsonAlbum
    {
        public string Name { get; set; }
        public int Playcount { get; set; }
        public Guid? Mbid { get; set; }
        public Uri Url { get; set; }
        public JsonAlbumArtist Artist { get; set; }
        public IList<JsonAlbumImage> Image { get; set; }
        [JsonProperty("@attr")]
        public JsonAlbumAttributes Attributes { get; set; }
    }

    public class JsonAlbumImage
    {
        [JsonProperty("#text")]
        public Uri Url { get; set; }
        public JsonAlbumImageSize Size { get; set; }
    }

    public class JsonAlbumAttributes
    {
        public int Rank { get; set; }
    }

    public class LastfmClient : ILastfmClient
    {
        private readonly string apiKey;
        private readonly string username;

        public LastfmClient(string apiKey, string username)
        {
            this.apiKey = apiKey;
            this.username = username;
        }

        public async Task<IList<JsonAlbum>> GetTopAlbumsForUser(string period, CancellationToken ctx)
        {
            var parameters = new Dictionary<string, object> {
                { "method", "user.gettopalbums" },
                { "user", username },
                { "api_key", apiKey },
                { "period", period },
                { "format", "json" }
            };

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(new Uri("https://ws.audioscrobbler.com/2.0/").AddQueryParameters(parameters));
                var json = JToken.Parse(response)["topalbums"]["album"];
                return json.ToObject<IList<JsonAlbum>>();
            }
        }
    }
}
