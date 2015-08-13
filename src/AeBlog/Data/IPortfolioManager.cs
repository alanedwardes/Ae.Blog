using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public interface IPortfolioManager
    {
        Task<Portfolio> GetPortfolioById(int id, CancellationToken ctx = default(CancellationToken));
        Task<IEnumerable<Portfolio>> GetPublishedPortfolios(CancellationToken ctx = default(CancellationToken));
        Task<IEnumerable<Portfolio>> GetFeaturedPortfolios(CancellationToken ctx = default(CancellationToken));
    }
}