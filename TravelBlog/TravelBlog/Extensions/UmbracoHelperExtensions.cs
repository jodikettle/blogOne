using System;
using System.Collections.Generic;
using System.Linq;
using TravelBlog.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Web;

namespace TravelBlog.Extensions
{
    public static class UmbracoHelperExtensions
    {
        public static PostsByTagModel GetContentByTag(this UmbracoHelper helper, IMasterModel masterModel, string tag, long page, long pageSize)
        {
            var appContext = helper.UmbracoContext.Application;
            var sqlSyntax = appContext.DatabaseContext.SqlSyntax;

            PostsByTagModel GetResult()
            {
                var sqlTags = GetTagQuery("umbracoNode.id", masterModel, sqlSyntax);
                if (sqlSyntax is MySqlSyntaxProvider)
                {
                    sqlTags.Where("cmsTags.tag = @tagName", new
                    {
                        tagName = tag
                    });
                }
                else
                {
                    //For whatever reason, SQLCE and even SQL SERVER are not willing to lookup 
                    //tags with hyphens in them, it's super strange, so we force the tag column to be - what it already is!! what tha.

                    sqlTags.Where("CAST(cmsTags.tag AS NVARCHAR(200)) = @tagName", new
                    {
                        tagName = tag
                    });
                }

                //get the publishedDate property type id on the ArticulatePost content type
                var publishedDatePropertyTypeId = appContext.DatabaseContext.Database.ExecuteScalar<int>(@"SELECT cmsPropertyType.id FROM cmsContentType
INNER JOIN cmsPropertyType ON cmsPropertyType.contentTypeId = cmsContentType.nodeId
WHERE cmsContentType.alias = @contentTypeAlias AND cmsPropertyType.alias = @propertyTypeAlias", new { contentTypeAlias = "BlogPost", propertyTypeAlias = "publishedDate" });

                var sqlContent = GetContentByTagQueryForPaging("umbracoNode.id", masterModel, sqlSyntax, publishedDatePropertyTypeId);

                sqlContent.Append("WHERE umbracoNode.id IN (").Append(sqlTags).Append(")");

                //order by the dataDate field which will be the publishedDate 
                sqlContent.OrderBy("cmsPropertyData.dataDate DESC");

                //TODO: ARGH This still returns multiple non distinct Ids :(

                var taggedContent = appContext.DatabaseContext.Database.Page<int>(page, pageSize, sqlContent);

                var result = new List<PostsByTagModel>();

                var publishedContent = helper.TypedContent(taggedContent.Items).WhereNotNull();

                var model = new PostsByTagModel(
                    publishedContent.Select(c => new PostListItemModel(c)),
                    tag,
                    "/Category/" + tag.ToLowerInvariant(),
                    Convert.ToInt32(taggedContent.TotalItems));

                result.Add(model);

                return result.FirstOrDefault();
            }

#if DEBUG
            return GetResult();
#else
            //cache this result for a short amount of time
            
            return (PostsByTagModel) appContext.ApplicationCache.RuntimeCache.GetCacheItem(
                string.Concat(typeof(UmbracoHelperExtensions).Name, "GetContentByTag", masterModel.RootBlogNode.Id, tagGroup, tag, page, pageSize),
                GetResult, TimeSpan.FromSeconds(30));
#endif
        }

        /// <summary>
        /// Gets the basic tag SQL used to retrieve tags for a given articulate root node
        /// </summary>
        /// <param name="selectCols"></param>
        /// <param name="masterModel"></param>
        /// <param name="sqlSyntax"></param>        
        /// <returns></returns>
        /// <remarks>
        /// TODO: We won't need this when this is fixed http://issues.umbraco.org/issue/U4-9290
        /// </remarks>
        private static Sql GetTagQuery(string selectCols, IMasterModel masterModel, ISqlSyntaxProvider sqlSyntax)
        {
            var sql = new Sql()
                .Select(selectCols)
                .From("cmsTags")
                .InnerJoin("cmsTagRelationship")
                .On("cmsTagRelationship.tagId = cmsTags.id")
                .InnerJoin("cmsContent")
                .On("cmsContent.nodeId = cmsTagRelationship.nodeId")
                .InnerJoin("umbracoNode")
                .On("umbracoNode.id = cmsContent.nodeId")
                .Where("umbracoNode.nodeObjectType = @nodeObjectType", new { nodeObjectType = Constants.ObjectTypes.Document })
                //only get nodes underneath the current articulate root
                .Where("umbracoNode." + sqlSyntax.GetQuotedColumnName("path") + " LIKE @path", new { path = masterModel.RootBlogNode.Path + ",%" });
            return sql;
        }

        /// <summary>
        /// Gets the tag SQL used to retrieve paged posts for particular tags for a given articulate root node
        /// </summary>
        /// <param name="selectCols"></param>
        /// <param name="masterModel"></param>
        /// <param name="sqlSyntax"></param>
        /// <param name="publishedDatePropertyTypeId">
        /// This is needed to perform the sorting on published date,  this is the PK of the property type for publishedDate on the ArticulatePost content type
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// TODO: We won't need this when this is fixed http://issues.umbraco.org/issue/U4-9290
        /// </remarks>
        private static Sql GetContentByTagQueryForPaging(string selectCols, IMasterModel masterModel, ISqlSyntaxProvider sqlSyntax, int publishedDatePropertyTypeId)
        {
            var sql = new Sql()
                .Select(selectCols)
                .From("umbracoNode")
                .InnerJoin("cmsDocument")
                .On("cmsDocument.nodeId = umbracoNode.id")
                .InnerJoin("cmsPropertyData")
                .On("cmsPropertyData.versionId = cmsDocument.versionId")
                .Where("umbracoNode.nodeObjectType = @nodeObjectType", new { nodeObjectType = Constants.ObjectTypes.Document })
                //Must be published, this will ensure there's only one version selected
                .Where("cmsDocument.published = 1")
                //must only return rows with the publishedDate property data so we only get one row and so we can sort on `cmsPropertyData.dataDate` which will be the publishedDate
                .Where("cmsPropertyData.propertytypeid = @propTypeId", new { propTypeId = publishedDatePropertyTypeId })
                //only get nodes underneath the current articulate root
                .Where("umbracoNode." + sqlSyntax.GetQuotedColumnName("path") + " LIKE @path", new { path = masterModel.RootBlogNode.Path + ",%" });
            return sql;
        }
    }
}