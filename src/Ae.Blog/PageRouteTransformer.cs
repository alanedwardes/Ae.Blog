using Ae.Blog.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace Ae.Blog
{
    public class PageRouteTransformer : DynamicRouteValueTransformer
    {
        private readonly IBlogPostRepository _repository;

        public PageRouteTransformer(IBlogPostRepository repository)
        {
            _repository = repository;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var content = await _repository.GetAllContentSummaries(httpContext.RequestAborted);

            if (values == null)
            {
                values = new RouteValueDictionary(new Dictionary<string, string> { { "page", "index" } });
            }

            if (content.Any(x => x.Slug == values["page"].ToString() && x.Type == Models.PostType.Page))
            {
                values["controller"] = "pages";
                values["action"] = "page";
            }

            return values;
        }
    }
}
