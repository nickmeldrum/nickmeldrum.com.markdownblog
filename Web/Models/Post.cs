using System.Web;

namespace MarkdownBlog.Net.Web.Models {
    public class Post : SiteViewModel
    {
        private readonly ContentItem _contentItem;

        public string Body { get { return _contentItem.Body; } }

        public PostMetadata Metadata { get { return (PostMetadata)_contentItem.Metadata; } }

        public Post(string postName, HttpContextWrapper httpContext) : base(httpContext)
        {
            _contentItem = new ContentItem(postName, Posts.PostsRoot, httpContext, Posts.List);
        }

        public Disqus Disqus
        {
            get { return new Disqus {ForumShortName = SiteData.DisqusShortName, PageIdentifier = Metadata.Slug}; }
        }
    }
}