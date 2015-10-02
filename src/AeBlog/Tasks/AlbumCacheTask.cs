using AeBlog.Caching;
using AeBlog.Clients;
using AeBlog.Data;
using AeBlog.Extensions;
using AeBlog.Options;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class AlbumCacheTask : IScheduledTask
    {
        private readonly ICacheProvider cacheProvider;
        private readonly ILastfmClientFactory lastfmFactory;
        private readonly IS3ClientFactory s3Factory;
        private readonly ILogger<AlbumCacheTask> logger;

        private static readonly string BucketName = "ae-lastfm-art";

        public AlbumCacheTask(ILastfmClientFactory lastfmFactory, IS3ClientFactory s3Factory, ICacheProvider cacheProvider, ILogger<AlbumCacheTask> logger)
        {
            this.s3Factory = s3Factory;
            this.cacheProvider = cacheProvider;
            this.lastfmFactory = lastfmFactory;
            this.logger = logger;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            var lastfmClient = lastfmFactory.CreateLastfmClient();

            var jsonAlbums = await lastfmClient.GetTopAlbumsForUser("7day", ctx);
            var s3Client = s3Factory.CreateS3Client();

            IList<Album> albums;

            using (var client = new HttpClient())
            {
                var tasks = jsonAlbums.Select(j => CacheAlbum(client, s3Client, j, ctx));

                albums = await Task.WhenAll(tasks);
            }

            await cacheProvider.Set("albums", albums.Where(a => a != null).ToList(), ctx);

            return TimeSpan.FromHours(1);
        }

        private async Task<Album> CacheAlbum(HttpClient httpClient, IAmazonS3 s3Client, JsonAlbum jsonAlbum, CancellationToken ctx)
        {
            var image = jsonAlbum.Image.Where(i => i.Size == JsonAlbumImageSize.Medium && i.Url != null).SingleOrDefault();
            if (image == null)
            {
                return null;
            }

            var album = new Album
            {
                Artist = jsonAlbum.Artist,
                Name = jsonAlbum.Name,
                Playcount = jsonAlbum.Playcount,
                Rank = jsonAlbum.Attributes.Rank,
                Url = jsonAlbum.Url
            };
            album.Thumbnail = new Uri("https://s3-eu-west-1.amazonaws.com/ae-lastfm-art/" + album.ToString().ToSlug());

            try
            {
                var meta = await s3Client.GetObjectMetadataAsync(BucketName, album.ToString().ToSlug(), ctx);
                logger.LogVerbose($"Found existing cache of art for {album}");
                return album;
            }
            catch (AmazonS3Exception ex)
            {
                if (!ex.ErrorCode.Equals("forbidden", StringComparison.OrdinalIgnoreCase))
                {
                    throw;
                }
            }

            string mediaType;
            byte[] imageBytes;
            try
            {
                logger.LogVerbose($"Downloading art for {album}");
                var response = await httpClient.GetAsync(image.Url, ctx);
                mediaType = response.Content.Headers?.ContentType?.MediaType;
                imageBytes = await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                // If anything happened here, log and continue. We don't want to retry.
                logger.LogWarning($"Hit exception whilst trying to download art for {album}", ex);
                return null;
            }

            PutObjectResponse putResponse;
            using (var ms = new MemoryStream(imageBytes))
            {
                logger.LogVerbose($"Storing art for {album}");
                putResponse = await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    ContentType = mediaType,
                    InputStream = ms,
                    Key = album.ToString().ToSlug(),
                    StorageClass = S3StorageClass.ReducedRedundancy,
                    CannedACL = S3CannedACL.PublicRead
                }, ctx);
            }

            return album;
        }
    }
}
