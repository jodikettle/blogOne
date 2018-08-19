using Umbraco.Core.Models;

namespace TravelBlog.Models
{
    public interface IMasterModel : IPublishedContent
    {
        IPublishedContent RootBlogNode { get; }

        int PageSize { get; }
        string DisqusShortName { get; }
        string CategoryName { get; }
        string PageTitle { get; }
        string PageDescription { get; }
    }
}
