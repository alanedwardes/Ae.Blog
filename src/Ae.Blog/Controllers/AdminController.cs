using Ae.Freezer;
using Ae.Freezer.Writers;
using Ae.Blog.Models;
using Ae.Blog.Models.Admin;
using Ae.Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;

namespace Ae.Blog.Controllers
{
    public class AdminController : Controller
    {
        private readonly IBlogPostRepository blogPostRetriever;
        private readonly IAmazonCloudFront cloudFrontClient;
        private readonly IConfiguration configuration;
        private readonly IAmazonS3 amazonS3;
        private readonly IFreezer freezer;

        public AdminController(IBlogPostRepository blogPostRetriever, IFreezer freezer, IAmazonCloudFront cloudFrontClient, IConfiguration configuration)
        {
            this.blogPostRetriever = blogPostRetriever;
            this.cloudFrontClient = cloudFrontClient;
            this.configuration = configuration;
            this.freezer = freezer;

            amazonS3 = new AmazonS3Client(RegionEndpoint.GetBySystemName(configuration["CDN_REGION"]));
        }

        public async Task<IActionResult> Index()
        {
            var distributionTask = cloudFrontClient.GetDistributionAsync(new GetDistributionRequest
            {
                Id = configuration["CLOUDFRONT_DISTRIBUTION"]
            }, CancellationToken.None);

            var summaries = blogPostRetriever.GetAllContentSummaries(CancellationToken.None);

            return View(new AdminModel{Posts = await summaries, Distribution = await distributionTask});
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var objectKey = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            await amazonS3.PutObjectAsync(new PutObjectRequest
            {
                BucketName = configuration["CDN_BUCKET"],
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead,
                Key = objectKey
            });

            return Redirect(string.Format(configuration["CDN_URI_FORMAT"], objectKey));
        }

        [HttpPost]
        public async Task<IActionResult> New(EditPostModel model)
        {
            var post = new Post
            {
                Content = model.Content,
                Type = model.Type,
                Title = model.Title,
                Category = model.Category,
                Published = DateTime.UtcNow,
                Slug = model.Slug
            };

            await blogPostRetriever.PutContent(post, CancellationToken.None);

            return Redirect(Url.Action(nameof(Index)));
        }

        [HttpGet]
        public IActionResult New()
        {
            return View("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditPostModel model)
        {
            var post = await blogPostRetriever.GetContent(id, CancellationToken.None);

            post.Content = model.Content;
            post.Category = model.Category;
            post.Updated = model.Type == PostType.Draft ? null : DateTime.UtcNow;
            post.Type = model.Type;
            post.Title = model.Title;

            await blogPostRetriever.PutContent(post, CancellationToken.None);

            return Redirect(Url.Action(nameof(Index)));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var post = await blogPostRetriever.GetContent(id, CancellationToken.None);

            return View(new EditPostModel
            {
                Title = post.Title,
                Category = post.Category,
                Content = post.Content,
                Type = post.Type
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await blogPostRetriever.DeleteContent(id, CancellationToken.None);
            return Redirect(Url.Action(nameof(Index)));
        }

        [HttpPost]
        public async Task<IActionResult> Publish()
        {
            var staticAssetPrefix = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["STATIC_ASSET_PREFIX"];

            var freezerConfiguration = new FreezerConfiguration
            {
                HttpClientName = "FREEZER_CLIENT",
                ResourceWriter = x => x.GetRequiredService<IWebsiteResourceWriter>()
            };
            freezerConfiguration.AdditionalResources.Add("sitemap.xml");
            freezerConfiguration.AdditionalResources.Add("blog/search");

            var staticAssets = "wwwroot";
            foreach (var staticAsset in Directory.EnumerateFiles(staticAssets, "*", SearchOption.AllDirectories))
            {
                var publicPath = staticAsset[staticAssets.Length..].Replace("\\", "/");
                freezerConfiguration.AdditionalResources.Add($"{staticAssetPrefix}{publicPath}");
            }

            foreach (var content in (await blogPostRetriever.GetAllContentSummaries(CancellationToken.None)))
            {
                if (content.Type == PostType.Page)
                {
                    var path = content.Url.TrimStart('/');
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        freezerConfiguration.AdditionalResources.Add(path);
                    }
                }

                if (content.Type == PostType.Published)
                {
                    freezerConfiguration.AdditionalResources.Add($"blog/posts/{content.Slug}.md");
                }
            }

            await freezer.Freeze(freezerConfiguration, CancellationToken.None);
            return Redirect(Url.Action(nameof(Index)));
        }

        [HttpPost]
        public async Task<IActionResult> Flush()
        {
            var request = new CreateInvalidationRequest
            {
                DistributionId = configuration["CLOUDFRONT_DISTRIBUTION"],
                InvalidationBatch = new InvalidationBatch
                {
                    CallerReference = $"Flush from aeblog {DateTimeOffset.UtcNow}",
                    Paths = new Paths
                    {
                        Items = new List<string> { "/*" },
                        Quantity = 1
                    }
                }
            };

            await cloudFrontClient.CreateInvalidationAsync(request);
            return Redirect(Url.Action(nameof(Index)));
        }
    }
}
