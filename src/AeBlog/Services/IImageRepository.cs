using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Services
{
    public interface IImageRepository
    {
        Task<Tuple<int, int>> GetImageDimensions(string url, CancellationToken token);
    }
}