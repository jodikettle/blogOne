using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace TravelBlog.Models
{
    public class MasterModel : PublishedContentWrapped, IMasterModel
    {
        private string _pageTitle;
        private string _pageDescription;

        public MasterModel(IPublishedContent content)
            : base(content) { }

        public IPublishedContent RootBlogNode => throw new System.NotImplementedException();

        public IPublishedContent BlogArchiveNode => throw new System.NotImplementedException();

        public IPublishedContent BlogAuthorsNode => throw new System.NotImplementedException();

        public string BlogTitle => throw new System.NotImplementedException();

        public string BlogDescription => throw new System.NotImplementedException();

        public string BlogLogo => throw new System.NotImplementedException();

        public string BlogBanner => throw new System.NotImplementedException();

        public int PageSize => throw new System.NotImplementedException();

        public string DisqusShortName => throw new System.NotImplementedException();

        public string CustomRssFeed => throw new System.NotImplementedException();

        public string PageTitle
        {
            get { return _pageTitle ?? (_pageTitle = Name + " - " + BlogTitle); }
            protected set { _pageTitle = value; }
        }

        public string PageDescription
        {
            get { return _pageDescription ?? (_pageDescription = BlogDescription); }
            protected set { _pageDescription = value; }
        }

        public string PageTags { get; protected set; }
    }
}