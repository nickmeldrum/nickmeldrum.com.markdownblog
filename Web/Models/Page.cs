using System.Web;

namespace MarkdownBlog.Net.Web.Models {
    public class Page : SiteViewModel {
        private readonly ContentItem _contentItem;

        public string Body { get { return _contentItem.Body; } }

        public ContentItemMetaData Metadata { get { return _contentItem.Metadata; } }

        public Page(string pageName, HttpContextWrapper httpContext) : base(httpContext) {
            _contentItem = new ContentItem(pageName, Pages.PagesRoot, httpContext, Pages.List);
        }
    }
}