using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface IPortfolioManager
    {
        Task<Portfolio> GetPortfolioById(int id, CancellationToken ctx);
        Task<IEnumerable<Portfolio>> GetPortfoliosBySkillSlug(string slug, CancellationToken ctx);
        Task<IEnumerable<Portfolio>> GetPublishedPortfolios(CancellationToken ctx);
        Task<IEnumerable<Portfolio>> GetFeaturedPortfolios(CancellationToken ctx);
    }
}