using System.Web;

namespace MarkdownBlog.Net.Web.Models {
    public class Post 
    {
        private readonly ContentItem _contentItem;

        public string Body { get { return _contentItem.Body; } }

        public PostMetadata Metadata { get { return (PostMetadata)_contentItem.Metadata; } }

        public Post(string postName, HttpContextWrapper httpContext)
        {
            _contentItem = new ContentItem(postName, PostsMetadata.PostsRoot, httpContext, PostsMetadata.Instance.List);
        }

        public Disqus Disqus
        {
            get { return new Disqus {ForumShortName = ((Site)HttpContext.Current.Application["SiteSettings"]).DisqusShortName, PageIdentifier = Metadata.Slug}; }
        }
    }
}