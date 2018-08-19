using System;
using System.Collections.Generic;
using System.Linq;
using TravelBlog.Extensions;
using Umbraco.Core;

namespace TravelBlog.Models
{
    public class PostsByTagModel
    {
        public PostsByTagModel(IEnumerable<PostListItemModel> posts, string tagName, string tagUrl)
    : this(posts, tagName, tagUrl, -1)
        {
        }

        public PostsByTagModel(IEnumerable<PostListItemModel> posts, string tagName, string tagUrl, int count)
        {
            if (posts == null) throw new ArgumentNullException(nameof(posts));
            if (tagName == null) throw new ArgumentNullException(nameof(tagName));
            if (tagUrl == null) throw new ArgumentNullException(nameof(tagUrl));

            //resolve to array so it doesn't double lookup
            Posts = posts.ToArray();
            TagName = tagName;
            var safeEncoded = tagUrl.SafeEncodeUrlSegments();
            TagUrl = safeEncoded.Contains("//") ? safeEncoded : safeEncoded.EnsureStartsWith('/');
            if (count > -1)
                _count = count;
        }

        public IEnumerable<PostListItemModel> Posts { get; }
        public string TagName { get; private set; }
        public string TagUrl { get; private set; }

        private int? _count;
        public int PostCount
        {
            get
            {
                if (_count.HasValue == false)
                {
                    _count = Posts.Count();
                }
                return _count.Value;
            }
        }
    }
}