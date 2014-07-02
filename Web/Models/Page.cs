
using HtmlAgilityPack;

namespace MarkdownBlog.Net.Web.Models {
    public class Page {
        private readonly ContentItem _contentItem;

        public string Body { get { return _contentItem.Body; } }
        public string BodyWithoutHtml {
            get {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Body);
                return htmlDoc.DocumentNode.InnerText;
            }
        }

        public ContentItemMetaData Metadata { get { return _contentItem.Metadata; } }

        public Page(string pageName)
            : this(pageName, PagesMetadata.Instance) {
        }

        public Page(string pageName, PagesMetadata metadataInstance) {
            _contentItem = new ContentItem(pageName, PagesMetadata.PagesRoot, metadataInstance.List);
        }


    }
}