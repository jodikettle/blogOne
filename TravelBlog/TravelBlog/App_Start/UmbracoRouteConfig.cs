using System.Web.Mvc;
using System.Web.Routing;
using TravelBlog.RouteHandler;
using Umbraco.Core;
using Umbraco.Web;

namespace TravelBlog.App_Start
{
    public class UmbracoRouteConfig : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var routes = RouteTable.Routes;
            var cache = UmbracoContext.Current.ContentCache;
            var urlProvider = UmbracoContext.Current.UrlProvider;

            //NOTE: need to write lock because this might need to be remapped while the app is running if
            // any articulate nodes are updated with new values
            using (routes.GetWriteLock())
            {
                RouteTable.Routes.MapUmbracoRoute(
                    "CategoryRoute",
                    "Category/{tag}",
                    new
                    {
                        controller = "CategoryList",
                        action = "ListByTag"
                    },
                    new CategoryRouteHandler(urlProvider, 1065));
            }
        }
    }
}