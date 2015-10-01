using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Banner { get; set; }
        public string Category { get; set; }
        public string Preview { get; set; }
        public string Description { get; set; }
        [JsonProperty("is_featured")]
        public bool IsFeatured { get; set; }
        [JsonProperty("is_published")]
        public bool IsPublished { get; set; }
        public DateTime? Published { get; set; }
        public IList<string> Skills { get; set; }
        public IList<string> Screenshots { get; set; }
    }

    public class PortfolioManager : IPortfolioManager
    {
        private const string PortfolioTable = "Aeblog.Portfolio";
        private const string HashKey = "id";
        private const string FeaturedKey = "is_featured";
        private const string FeaturedIndex = "is_featured-index";
        private const string PublishedKey = "is_published";
        private const string PublishedIndex = "is_published-index";

        private readonly IDocumentStore documentStore;

        public PortfolioManager(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public async Task<Portfolio> GetPortfolioById(int id, CancellationToken ctx = default(CancellationToken))
        {
            return (await documentStore.GetItems<Portfolio>(PortfolioTable, HashKey, id, null, ctx)).SingleOrDefault();
        }

        public Task<IEnumerable<Portfolio>> GetFeaturedPortfolios(CancellationToken ctx = default(CancellationToken))
        {
            return documentStore.GetItems<Portfolio>(PortfolioTable, FeaturedKey, 1, FeaturedIndex, ctx);
        }

        public Task<IEnumerable<Portfolio>> GetPublishedPortfolios(CancellationToken ctx = default(CancellationToken))
        {
            return documentStore.GetItems<Portfolio>(PortfolioTable, PublishedKey, 1, PublishedIndex, ctx);
        }
    }
}
