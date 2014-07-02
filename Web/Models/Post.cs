using System.Web;
using HtmlAgilityPack;

namespace MarkdownBlog.Net.Web.Models {
    public class Post 
    {
        private readonly ContentItem _contentItem;

        public string Body { get { return _contentItem.Body; } }
        public string BodyWithoutHtml {
            get {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Body);
                return htmlDoc.DocumentNode.InnerText;
            }
        }

        public PostMetadata Metadata { get { return (PostMetadata)_contentItem.Metadata; } }

        public Post(string postName) : this(postName, PostsMetadata.Instance) {
        }

        public Post(string postName, PostsMetadata metadataInstance) {
            _contentItem = new ContentItem(postName, PostsMetadata.PostsRoot, metadataInstance.List);
        }

        public Disqus Disqus
        {
            get { return new Disqus {ForumShortName = ((Site)HttpContext.Current.Application["SiteSettings"]).DisqusShortName, PageIdentifier = Metadata.Slug}; }
        }
    }
}