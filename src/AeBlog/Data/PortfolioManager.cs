using AeBlog.Caching;
using AeBlog.Extensions;
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
        public static string TableName { get; } = "Aeblog.Portfolio";

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
        private readonly ICacheProvider cacheProvider;

        public PortfolioManager(ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        private async Task<IEnumerable<Portfolio>> GetPortfolios(CancellationToken ctx)
        {
            return await cacheProvider.Get<IList<Portfolio>>(Portfolio.TableName, ctx) ?? Enumerable.Empty<Portfolio>();
        }

        public async Task<Portfolio> GetPortfolioById(int id, CancellationToken ctx)
        {
            return (await GetPortfolios(ctx)).Where(p => p.Id == id && p.IsPublished).SingleOrDefault();
        }

        public async Task<IEnumerable<Portfolio>> GetFeaturedPortfolios(CancellationToken ctx)
        {
            return (await GetPortfolios(ctx)).Where(p => p.IsFeatured && p.IsPublished);
        }

        public async Task<IEnumerable<Portfolio>> GetPublishedPortfolios(CancellationToken ctx)
        {
            return (await GetPortfolios(ctx)).Where(p => p.IsPublished);
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosBySkillSlug(string slug, CancellationToken ctx)
        {
            return (await GetPortfolios(ctx)).Where(p => p.Skills.Any(s => s.ToSlug() == slug) && p.IsPublished);
        }
    }
}
