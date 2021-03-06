﻿using Ae.Blog.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Ae.Blog.Services
{
    public interface IBlogPostRepository
    {
        Task<Post[]> GetPublishedPosts(CancellationToken token);
        Task<PostSummary[]> GetAllPostSummaries(CancellationToken token);
        Task<Post> GetPost(string slug, CancellationToken token);
        Task PutPost(Post post, CancellationToken token);
        Task DeletePost(string slug, CancellationToken token);
    }
}