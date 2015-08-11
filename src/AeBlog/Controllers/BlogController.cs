using AeBlog.Data;
using AeBlog.ViewModels;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    [Route("")]
    public class BlogController : Controller
    {
        private readonly IPostManager postManager;
        private readonly ILastfmDataProvider lastfmDataProvider;

        public BlogController(IPostManager postManager, ILastfmDataProvider lastfmDataProvider)
        {
            this.postManager = postManager;
            this.lastfmDataProvider = lastfmDataProvider;
        }

        [Route("/")]
        public async Task<IActionResult> Home(CancellationToken ctx = default(CancellationToken))
        {
            var albums = await lastfmDataProvider.GetTopAlbumsForUser("alanedwardes", "", "7day", ctx);

            var posts = await postManager.GetPosts(ctx);

            return View(new HomeViewModel(posts, albums));
        }

        [Route("/archive/")]
        public async Task<IActionResult> Archive(CancellationToken ctx = default(CancellationToken))
        {
            var posts = await postManager.GetPosts(ctx);

            var published = posts.Where(p => p.IsPublished);

            return View(new ArchiveViewModel(published));
        }

        [Route("/posts/{slug}/")]
        public async Task<IActionResult> SinglePost(string slug, CancellationToken ctx = default(CancellationToken))
        {
            var post = await postManager.GetPostBySlug(slug, ctx);
            if (post == null)
            {
                return HttpNotFound();
            }

            if (!post.IsPublished)
            {
                return HttpNotFound();
            }

            return View(new SinglePostViewModel(post));
        }

        [Route("/errors/{code}")]
        public IActionResult Error(int code)
        {
            switch (code)
            {
                case 404:
                    return View(new ErrorViewModel("Not Found", "Sorry, the page you were looking for wasn't found."));
                case 500:
                    return View(new ErrorViewModel("Internal Server Error", "Sorry, this page encountered a problem."));
                default:
                    return View(new ErrorViewModel($"Error {code}", "Sorry, this page encountered a problem."));
            }
        }
    }
}
