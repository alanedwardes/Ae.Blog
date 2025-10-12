using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Ae.Blog.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ae.Blog.Services
{
	public class LocalBlogPostRepository : IBlogPostRepository
	{
		private readonly ILogger<LocalBlogPostRepository> logger;
		private readonly IConfiguration configuration;
		private readonly string postsPath;
		private readonly JsonSerializerOptions jsonOptions;
		private Task<Post[]> allPosts;

		public LocalBlogPostRepository(ILogger<LocalBlogPostRepository> logger, IConfiguration configuration)
		{
			this.logger = logger;
			this.configuration = configuration;
			postsPath = configuration["POSTS_PATH"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Posts");
			jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				WriteIndented = true,
				Converters = { new JsonStringEnumConverter() }
			};
			ReloadPosts();
		}

		private void ReloadPosts()
		{
			allPosts = GetAllPosts(CancellationToken.None);
		}

		public async Task<Post[]> GetPublishedPosts(CancellationToken token)
		{
			return (await allPosts).Where(x => x.Type == PostType.Featured || x.Type == PostType.Published || x.Type == PostType.Demoted).ToArray();
		}

		public async Task<PostSummary[]> GetAllContentSummaries(CancellationToken token)
		{
			return await allPosts;
		}

		public async Task<Post> GetContent(string slug, CancellationToken token)
		{
			return (await allPosts).Single(x => x.Slug == slug);
		}

		public async Task PutContent(Post post, CancellationToken token)
		{
			Directory.CreateDirectory(postsPath);
			var path = Path.Combine(postsPath, $"{post.Slug}.json");
			var json = JsonSerializer.Serialize(post, jsonOptions);
			await File.WriteAllTextAsync(path, json, token);
			ReloadPosts();
		}

		public async Task DeleteContent(string slug, CancellationToken token)
		{
			var path = Path.Combine(postsPath, $"{slug}.json");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			ReloadPosts();
			await Task.CompletedTask;
		}

		private async Task<Post[]> GetAllPosts(CancellationToken token)
		{
			logger.LogInformation("Begin pulling posts from disk at {Path}", postsPath);
			if (!Directory.Exists(postsPath))
			{
				return Array.Empty<Post>();
			}

			var files = Directory.EnumerateFiles(postsPath, "*.json", SearchOption.TopDirectoryOnly).ToArray();
			var posts = new List<Post>(files.Length);

			foreach (var file in files)
			{
				token.ThrowIfCancellationRequested();
				var json = await File.ReadAllTextAsync(file, token);
				var post = JsonSerializer.Deserialize<Post>(json, jsonOptions);
				if (post == null) continue;

				post.ContentRaw = post.Content;

				if (!string.IsNullOrEmpty(post.Content))
				{
					post.Content = post.Content.Replace("$CDN_DOMAIN$", Constants.CDN_ROOT.ToString())
											   .Replace("$STATIC_ASSET_PREFIX$", configuration["STATIC_ASSET_PREFIX"]);
					post.PreCompute();
				}

				posts.Add(post);
			}

			logger.LogInformation("Retrieved {PostCount} posts from disk", posts.Count);
			return posts.OrderByDescending(x => x.Published).ToArray();
		}
	}
}

