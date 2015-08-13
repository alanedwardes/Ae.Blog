using AeBlog.Data;

namespace AeBlog.ViewModels
{
    public class SinglePortfolioViewModel
    {
        public SinglePortfolioViewModel(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public Portfolio Portfolio { get; }
    }
}
