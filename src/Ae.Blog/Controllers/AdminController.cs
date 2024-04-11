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
using System.Linq;

namespace Ae.Blog.Controllers
{
    public class AdminController : Controller
    {
        private readonly IBlogPostRepository blogPostRetriever;
        private readonly IAmazonCloudFront cloudFrontClient;
        private readonly IConfiguration configuration;
        private readonly IFreezer freezer;

        public AdminController(IBlogPostRepository blogPostRetriever, IFreezer freezer, IAmazonCloudFront cloudFrontClient, IConfiguration configuration)
        {
            this.blogPostRetriever = blogPostRetriever;
            this.cloudFrontClient = cloudFrontClient;
            this.configuration = configuration;
            this.freezer = freezer;
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
        public async Task<IActionResult> New(EditPostModel model)
        {
            var post = new Post
            {
                ContentRaw = model.Content,
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

            post.ContentRaw = model.Content;
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
            var freezerConfiguration = new FreezerConfiguration
            {
                HttpClientName = "FREEZER_CLIENT",
                ResourceWriter = x => x.GetRequiredService<IWebsiteResourceWriter>()
            };
            freezerConfiguration.AdditionalResources.Add(new Uri("sitemap.xml", UriKind.Relative));
            freezerConfiguration.AdditionalResources.Add(new Uri("lib/highlight/atom-one-dark.min.css", UriKind.Relative));
            freezerConfiguration.AdditionalResources.Add(new Uri("blog/search", UriKind.Relative));

            foreach (var page in (await blogPostRetriever.GetAllContentSummaries(CancellationToken.None)).Where(x => x.Type == PostType.Page))
            {
                var path = page.Url.TrimStart('/');
                if (!string.IsNullOrWhiteSpace(path))
                {
                    freezerConfiguration.AdditionalResources.Add(new Uri(path, UriKind.Relative));
                }
            }

            freezerConfiguration.AdditionalResources.Add(new Uri("lib/model-viewer/model-viewer.min.js", UriKind.Relative));

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
