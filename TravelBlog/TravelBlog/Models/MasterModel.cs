using System;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace TravelBlog.Models
{
    public class MasterModel : PublishedContentWrapped, IMasterModel
    {
        private IPublishedContent _rootBlogNode;
        private string _pageTitle;
        private string _category;
        private string _pageDescription;
        private int? _pageSize;
        private string _disqusShortName;

        public MasterModel(IPublishedContent content)
            : base(content) { }

        public IPublishedContent RootBlogNode
        {
            get
            {
                var root = Content.AncestorOrSelf("blog");
                if (root == null)
                {
                    throw new InvalidOperationException("Could not find the Blog root document for the current rendered page");
                }
                _rootBlogNode = root;
                return _rootBlogNode;
            }
            protected set { _rootBlogNode = value; }
        }


        public string PageDescription
        {
            get
            {
                _pageDescription = Content.GetPropertyValue<string>("Description", true);
                return _pageDescription;
            }
            protected set { _pageDescription = value; }
        }

        public int PageSize
        {
            get
            {
                if (_pageSize.HasValue == false)
                {
                    _pageSize = Content.GetPropertyValue<int>("pageSize", true, 10);
                }
                return _pageSize.Value;
            }
            protected set { _pageSize = value; }
        }

        public string DisqusShortName
        {
            get { return _disqusShortName ?? (_disqusShortName = Content.GetPropertyValue<string>("disqusShortname", true)); }
            protected set { _disqusShortName = value; }
        }

        public string PageTitle
        {
            get { return _pageTitle ?? (_pageTitle = Name); }
            protected set { _pageTitle = value; }
        }

        public string CategoryName
        {
            get { return _category ?? (_category = Name); }
            protected set { _category = value; }
        }
    }
}