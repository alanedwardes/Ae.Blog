using Ae.Freezer;
using Ae.Freezer.Writers;
using Ae.Blog.Models;
using Ae.Blog.Models.Admin;
using Ae.Blog.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using System.Collections.Generic;

namespace Ae.Blog.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class AdminController : Controller
    {
        private readonly IBlogPostRepository blogPostRetriever;
        private readonly IAmazonCloudFront cloudFrontClient;
        private readonly IFreezer freezer;

        public AdminController(IBlogPostRepository blogPostRetriever, IFreezer freezer, IAmazonCloudFront cloudFrontClient)
        {
            this.blogPostRetriever = blogPostRetriever;
            this.cloudFrontClient = cloudFrontClient;
            this.freezer = freezer;
        }

        public async Task<IActionResult> Index()
        {
            var summaries = await blogPostRetriever.GetAllPostSummaries(CancellationToken.None);

            return View(new AdminModel{Posts = summaries});
        }

        [AllowAnonymous]
        public IActionResult Login() => Challenge();

        [AllowAnonymous]
        public IActionResult Denied() => Content("Unathorized");

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect(Url.Action(nameof(Index), nameof(HomeController)));
        }

        [HttpPost]
        public async Task<IActionResult> New(EditPostModel model)
        {
            var post = new Post
            {
                Content = model.Content,
                Type = model.IsPublished ? "published" : "draft",
                Title = model.Title,
                Category = model.Category,
                Published = DateTime.UtcNow,
                Slug = model.Slug
            };

            await blogPostRetriever.PutPost(post, CancellationToken.None);

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
            var post = await blogPostRetriever.GetPost(id, CancellationToken.None);

            post.Content = model.Content;
            post.Category = model.Category;
            post.Updated = model.IsPublished ? DateTime.UtcNow : (DateTime?)null;
            post.Type = model.IsPublished ? "published" : "draft";
            post.Title = model.Title;

            await blogPostRetriever.PutPost(post, CancellationToken.None);

            return Redirect(Url.Action(nameof(Index)));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var post = await blogPostRetriever.GetPost(id, CancellationToken.None);

            return View(new EditPostModel
            {
                Title = post.Title,
                Category = post.Category,
                Content = post.Content,
                IsPublished = post.Type == "published"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await blogPostRetriever.DeletePost(id, CancellationToken.None);
            return Redirect(Url.Action(nameof(Index)));
        }

        [HttpPost]
        public async Task<IActionResult> Publish()
        {
            var freezerConfiguration = new FreezerConfiguration
            {
                BaseAddress = new Uri("https://uncached.alanedwardes.com"),
                ResourceWriter = x => x.GetRequiredService<IWebsiteResourceWriter>()
            };
            freezerConfiguration.AdditionalResources.Add(new Uri("sitemap.xml", UriKind.Relative));
            freezerConfiguration.AdditionalResources.Add(new Uri("lib/highlight/atom-one-dark.min.css", UriKind.Relative));

            await freezer.Freeze(freezerConfiguration, CancellationToken.None);
            return Redirect(Url.Action(nameof(Index)));
        }

        [HttpPost]
        public async Task<IActionResult> Flush()
        {
            var request = new CreateInvalidationRequest
            {
                DistributionId = Environment.GetEnvironmentVariable("CLOUDFRONT_DISTRIBUTION"),
                InvalidationBatch = new InvalidationBatch
                {
                    CallerReference = $"Flush from aeblog {DateTimeOffset.UtcNow}",
                    Paths = new Paths
                    {
                        Items = new List<string> { "*" },
                        Quantity = 1
                    }
                }
            };

            await cloudFrontClient.CreateInvalidationAsync(request);
            return Redirect(Url.Action(nameof(Index)));
        }
    }
}
