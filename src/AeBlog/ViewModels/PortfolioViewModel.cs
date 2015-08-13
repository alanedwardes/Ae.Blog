using System.Collections.Generic;
using AeBlog.Data;

namespace AeBlog.ViewModels
{
    public class PortfolioViewModel
    {
        public PortfolioViewModel(IEnumerable<Portfolio> portfolios, IEnumerable<Category> categories)
        {
            Portfolios = portfolios;
            Categories = categories;
        }

        public IEnumerable<Category> Categories { get; }
        public IEnumerable<Portfolio> Portfolios { get; }
    }
}
