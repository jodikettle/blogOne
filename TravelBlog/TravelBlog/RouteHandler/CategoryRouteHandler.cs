using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using TravelBlog.Models;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;

namespace TravelBlog.RouteHandler
{
    public class CategoryRouteHandler : UmbracoVirtualNodeRouteHandler
    {
        private IPublishedContent _blogRootNode { get; set; }

        /// <summary>
        /// Constructor used to create a new handler for multi-tenency with domains and ids
        /// </summary>
        /// <param name="itemsForRoute"></param>
        public CategoryRouteHandler(UrlProvider umbracoUrlProvider, int blogId)
        {
            _blogRootNode = UmbracoContext.Current.ContentCache.GetById(blogId);
        }

        protected override IPublishedContent FindContent(RequestContext requestContext, UmbracoContext umbracoContext)
        {
            var baseContent = new { Id = _blogRootNode };
            var tag = requestContext.RouteData.GetRequiredString("tagName");
            var urlName = "Category";

            return new VirtualPage(
                _blogRootNode,
                tag,
                requestContext.RouteData.GetRequiredString("controller"),
                tag.IsNullOrWhiteSpace()
                    ? urlName
                    : urlName.EnsureEndsWith('/') + tag);
        }
    }
}