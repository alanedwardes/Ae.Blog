using AeBlog.Models;
using System.Threading.Tasks;

namespace AeBlog.Services
{
    public interface ICloudFrontInvalidator
    {
        Task InvalidatePost(PostSummary postSummary);
    }
}