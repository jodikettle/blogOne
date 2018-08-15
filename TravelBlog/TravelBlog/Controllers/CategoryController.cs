using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelBlog.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Core;
using Umbraco.Web;
using TravelBlog.Extensions;

namespace TravelBlog.Controllers
{
    public class CategoryListController : ListControllerBase
    {
        public ActionResult ListByTag(RenderModel model, string tag, int? p)
        {
            // in my case, the IPublishedContent attached to this
            // model will be my products node in Umbraco which i
            // can now use to traverse to display the product list
            // or lookup the product by sku

            if (string.IsNullOrEmpty(tag))
            {
                // render the products list if no sku
                return RedirectToRoute("/");
            }
            else
            {
                return RenderArticleListByCategory(model, tag, p);
            }
        }

        private ActionResult RenderArticleListByCategory(IRenderModel model, string tag, int? p)
        {
            var tagurlName = model.Content.GetPropertyValue<string>("tagsUrlName");

            var tagPage = model.Content as VirtualPage;
            if (tagPage == null)
            {
                throw new InvalidOperationException("The RenderModel.Content instance must be of type " + typeof(VirtualPage));
            }

            //create a master model
            var masterModel = new MasterModel(model.Content);

            var contentByTag = Umbraco.GetContentByTag(masterModel, tagPage.Name, tagurlName, p ?? 1, masterModel.PageSize);

            //this is a special case in the event that a tag contains a '.', when this happens we change it to a '-'
            // when generating the URL. So if the above doesn't return any tags and the tag contains a '-', then we
            // will replace them with '.' and do the lookup again
            if ((contentByTag == null || contentByTag.PostCount == 0) && tagPage.Name.Contains("-"))
            {
                contentByTag = Umbraco.GetContentByTag(
                    masterModel,
                    tagPage.Name.Replace('-', '.'),
                    tagurlName,
                    p ?? 1, masterModel.PageSize);
            }

            if (contentByTag == null)
            {
                return new HttpNotFoundResult();
            }

            return GetPagedListView(masterModel, tagPage, contentByTag.Posts, contentByTag.PostCount, p);
        }
    }
}