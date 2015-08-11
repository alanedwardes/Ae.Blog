using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public enum AlbumImageSize
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public class AlbumArtist
    {
        public string Name { get; set; }
        public Guid? mbid { get; set; }
        public Uri Url { get; set; }
    }

    public class Album
    {
        public string Name { get; set; }
        public int Playcount { get; set; }
        public Guid? Mbid { get; set; }
        public Uri Url { get; set; }
        public AlbumArtist Artist { get; set; }
        public IList<AlbumImage> Image { get; set; }
        [JsonProperty("@attr")]
        public AlbumAttributes Attributes { get; set; }
    }

    public class AlbumImage
    {
        [JsonProperty("#text")]
        public Uri Url { get; set; }
        public AlbumImageSize Size { get; set; }
    }

    public class AlbumAttributes
    {
        public int Rank { get; set; }
    }

    public class TopAlbums
    {
        [JsonProperty("album")]
        public IList<Album> Albums { get; set; }
    }

    public class TopAlbumsResponse
    {
        public TopAlbums TopAlbums { get; set; }
    }

    public class LastfmDataProvider : ILastfmDataProvider
    {
        private readonly ILogger<LastfmDataProvider> logger;

        public LastfmDataProvider(ILogger<LastfmDataProvider> logger)
        {
            this.logger = logger;
        }

        public async Task<IEnumerable<Album>> GetTopAlbumsForUser(string user, string api_key, string period, CancellationToken ctx = default(CancellationToken))
        {
            var json = string.Empty;

            using (var client = new HttpClient())
            {
                try
                {
                    json = await client.GetStringAsync(new Uri("https://ws.audioscrobbler.com/2.0/").AddQueryParameters(new Dictionary<string, object> {
                        { "method", "user.gettopalbums" },
                        { "user", user },
                        { "api_key", api_key },
                        { "period", period },
                        { "format", "json" }
                    }));
                }
                catch (HttpRequestException ex)
                {
                    logger.LogError("Error getting last.fm albums", ex);
                    return Enumerable.Empty<Album>();
                }
            }

            try
            {
                return JsonConvert.DeserializeObject<TopAlbumsResponse>(json)?.TopAlbums?.Albums ?? Enumerable.Empty<Album>();
            }
            catch (JsonSerializationException ex)
            {
                logger.LogError("Error deserialising last.fm albums", ex);
                return Enumerable.Empty<Album>();
            }
        }
    }
}
