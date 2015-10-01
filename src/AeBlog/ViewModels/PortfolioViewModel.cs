using System.Collections.Generic;
using AeBlog.Data;

namespace AeBlog.ViewModels
{
    public class PortfolioViewModel
    {
        public PortfolioViewModel(IEnumerable<Portfolio> portfolios)
        {
            Portfolios = portfolios;
        }

        public IEnumerable<Portfolio> Portfolios { get; }
    }
}
