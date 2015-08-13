using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Ordering { get; set; }
    }

    public class CategoryManager : ICategoryManager
    {
        private const string CategoryTable = "Aeblog.Category";
        private const string PublishedKey = "is_published";
        private const string PublishedIndex = "is_published-index";

        private readonly IDocumentStore documentStore;

        public CategoryManager(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public Task<IEnumerable<Category>> GetPublishedCategories(CancellationToken ctx = default(CancellationToken))
        {
            return documentStore.GetItems<Category>(CategoryTable, PublishedKey, 1, PublishedIndex, ctx);
        }
    }
}
