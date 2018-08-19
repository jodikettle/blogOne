using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace TravelBlog.Models
{
    public class ListPageModel : MasterModel
    {
        private readonly IEnumerable<PostListItemModel> _listItems;
        private IEnumerable<PostListItemModel> _resolvedList;
        private readonly PagerModel _pager;

        public ListPageModel(IPublishedContent content, IEnumerable<PostListItemModel> listItems, PagerModel pager)
            : base(content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (listItems == null) throw new ArgumentNullException(nameof(listItems));
            if (pager == null) throw new ArgumentNullException(nameof(pager));

            _listItems = listItems;
            _pager = pager;
        }

        /// <summary>
        /// The pager model
        /// </summary>
        public PagerModel Pages => _pager;

        /// <summary>
        /// Strongly typed access to the list of blog posts
        /// </summary>
        public IEnumerable<PostListItemModel> Posts
        {
            get
            {

                if (_resolvedList != null)
                    return _resolvedList;

                if (_listItems == null)
                {
                    _resolvedList = base.Children.Select(x => new PostListItemModel(x)).ToArray();
                    return _resolvedList;
                }


                if (_listItems != null && _pager != null)
                {
                    _resolvedList = _listItems
                    //Skip will already be done in this case, but we'll take again anyways just to be safe                    
                        .Take(_pager.PageSize)
                        .Select(x => new PostListItemModel(x))
                        .ToArray();
                }
                else
                {
                    _resolvedList = Enumerable.Empty<PostListItemModel>();
                }

                return _resolvedList;
            }
        }

        /// <summary>
        /// The list of blog posts
        /// </summary>
        public override IEnumerable<IPublishedContent> Children => Posts;
    }
}