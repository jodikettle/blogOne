using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace TravelBlog.Models
{
    public class PostModel : MasterModel
    {

        public PostModel(IPublishedContent content)
            : base(content)
        {
            PageTitle = Name + " - " + BlogTitle;
            PageDescription = Excerpt;
            PageTags = string.Join(",", Tags);
        }

        public IEnumerable<string> Tags
        {
            get
            {
                if (!UmbracoConfig.For.UmbracoSettings().Content.EnablePropertyValueConverters)
                {
                    var tags = this.GetPropertyValue<string>("tags");
                    return tags.IsNullOrWhiteSpace() ? Enumerable.Empty<string>() : tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    var tags = this.GetPropertyValue<IEnumerable<string>>("tags");
                    return tags ?? Enumerable.Empty<string>();
                }
            }
        }

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

        public bool EnableComments => Content.GetPropertyValue<bool>("enableComments", true);

        public string Excerpt => this.GetPropertyValue<string>("excerpt");

        public DateTime PublishedDate => Content.GetPropertyValue<DateTime>("publishedDate");

        /// <summary>
        /// Some blog post may have an associated image
        /// </summary>
        public string PostImageUrl => Content.GetPropertyValue<string>("postImage");

        /// <summary>
        /// Cropped version of the PostImageUrl
        /// </summary>
        public string CroppedPostImageUrl => !PostImageUrl.IsNullOrWhiteSpace()
            ? this.GetCropUrl("postImage", "wide")
            : null;

        /// <summary>
        /// Social Meta Description
        /// </summary>
        public string SocialMetaDescription => this.GetPropertyValue<string>("socialDescription");

        public IHtmlString Body
        {
            get
            {
                if (this.HasProperty("richText"))
                {
                    return this.GetPropertyValue<IHtmlString>("richText");
                }
                else
                {
                    var val = this.GetPropertyValue<string>("markdown");
                    //var md = new MarkdownDeep.Markdown();
                    //UmbracoConfig.For.ArticulateOptions().MarkdownDeepOptionsCallBack(md);
                    //return new MvcHtmlString(md.Transform(val));
                    return new MvcHtmlString(val);
                }

            }
        }

        public string ExternalUrl => this.GetPropertyValue<string>("externalUrl");
    }
}