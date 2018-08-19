using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace TravelBlog.Models
{
    public class PostListItemModel: MasterModel
    {
        private readonly UrlProvider umbracoUrlProvider;
        private readonly UmbracoHelper umbracoHelper;

        public PostListItemModel(IPublishedContent content)
            : base(content)
        {
            umbracoUrlProvider = UmbracoContext.Current.UrlProvider;
            umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
        }
        public string Title { get; set; }

        public DateTime PublishedDate => Content.GetPropertyValue<DateTime>("publishedDate");

        public IEnumerable<string> Categories
        {
            get
            {
                if (!UmbracoConfig.For.UmbracoSettings().Content.EnablePropertyValueConverters)
                {
                    var tags = this.GetPropertyValue<string>("categories");
                    return tags.IsNullOrWhiteSpace() ? Enumerable.Empty<string>() : tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    var tags = this.GetPropertyValue<IEnumerable<string>>("categories");
                    return tags ?? Enumerable.Empty<string>();
                }
            }
        }

        public string Excerpt => this.GetPropertyValue<string>("excerpt");

        public string ImageUrl
        {
            get
            {
                var imageId = this.GetPropertyValue<string>("image");
                var content = umbracoHelper.Media(imageId);
                return content.Url;
            }
        }
    }
}