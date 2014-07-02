using System.Collections.Generic;
using System.Web;

namespace MarkdownBlog.Net.Web.Models {
    public class SiteViewModel {
        public PostsMetadata PostsMetadata { get { return _postsMetadata ?? (_postsMetadata = new PostsMetadata(new ContentItemsMetaData<PostMetadata>(HttpContext))); } }
        public PagesMetadata PagesMetadata { get { return _pagesMetadata ?? (_pagesMetadata = new PagesMetadata(new ContentItemsMetaData<ContentItemMetaData>(HttpContext))); } }
        public Site SiteData { get { return new Site(); } }

        public Dictionary<string, object> AsidesViewModels {
            get {
                return new Dictionary<string, object> {
                    { "StackOverflowFlair", new StackOverflowFlairViewModel(new StackOverflowFlair()) },
                    { "TwitterTimeline", new TwitterTimeline() },
                    { "ArchiveLinks", PostsMetadata.MonthlyArchiveLinks },
                };
            }
        } 

        public SiteViewModel(HttpContextWrapper httpContext) {
            HttpContext = httpContext;
        }

        protected readonly HttpContextWrapper HttpContext;
        private PostsMetadata _postsMetadata;
        private PagesMetadata _pagesMetadata;
    }
}
