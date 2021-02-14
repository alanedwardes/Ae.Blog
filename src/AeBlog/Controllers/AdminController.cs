﻿using Ae.Freezer;
using Ae.Freezer.Aws;
using Ae.Freezer.Writers;
using AeBlog.Models;
using AeBlog.Models.Admin;
using AeBlog.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class AdminController : Controller
    {
        private readonly IBlogPostRepository blogPostRetriever;
        private readonly IFreezer freezer;

        public AdminController(IBlogPostRepository blogPostRetriever, IFreezer freezer)
        {
            this.blogPostRetriever = blogPostRetriever;
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
        public async Task<IActionResult> Flush()
        {
            await freezer.Freeze(new FreezerConfiguration
            {
                BaseAddress = new Uri("https://uncached.alanedwardes.com"),
                ResourceWriter = x => x.GetRequiredService<IWebsiteResourceWriter>()
            }, CancellationToken.None);
            return Redirect(Url.Action(nameof(Index)));
        }
    }
}