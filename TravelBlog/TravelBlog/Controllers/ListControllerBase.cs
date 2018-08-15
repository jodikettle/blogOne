using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TravelBlog.Models;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace TravelBlog.Controllers
{
    public class ListControllerBase : RenderMvcController
    {
        /// <summary>
        /// Gets a paged list view for a given posts by author/tags/categories model
        /// </summary>
        protected ActionResult GetPagedListView(IMasterModel masterModel, IPublishedContent pageNode, IEnumerable<IPublishedContent> listItems, int totalPosts, int? p)
        {
            if (masterModel == null) throw new ArgumentNullException(nameof(masterModel));
            if (pageNode == null) throw new ArgumentNullException(nameof(pageNode));
            if (listItems == null) throw new ArgumentNullException(nameof(listItems));

            PagerModel pager;
            if (!GetPagerModel(masterModel, totalPosts, p, out pager))
            {
                return new RedirectToUmbracoPageResult(masterModel.RootBlogNode, UmbracoContext);
            }

            var listModel = new ListModel(pageNode, listItems, pager);

            return View(PathHelper.GetThemeViewPath(listModel, "List"), listModel);
        }

        protected bool GetPagerModel(IMasterModel masterModel, int totalPosts, int? p, out PagerModel pager)
        {
            if (p == null || p.Value <= 0)
            {
                p = 1;
            }

            var pageSize = masterModel.PageSize;
            var totalPages = totalPosts == 0 ? 1 : Convert.ToInt32(Math.Ceiling((double)totalPosts / pageSize));

            //Invalid page, redirect without pages
            if (totalPages < p)
            {
                pager = null;
                return false;
            }

            pager = new PagerModel(
                pageSize,
                p.Value - 1,
                totalPages,
                totalPages > p ? masterModel.Url.EnsureEndsWith('?') + "p=" + (p + 1) : null,
                p > 2 ? masterModel.Url.EnsureEndsWith('?') + "p=" + (p - 1) : p > 1 ? masterModel.Url : null);

            return true;
        }
    }
}