using System.Collections.Generic;
using System.Web;

namespace MarkdownBlog.Net.Web.Models {
    public class SiteViewModel {
        public Posts Posts { get { return _posts ?? (_posts = new Posts(HttpContext)); } }
        public Site SiteData { get { return new Site(); } }

        public Dictionary<string, object> AsidesViewModels {
            get {
                return new Dictionary<string, object> {
                    { "StackOverflowFlair", new StackOverflowFlairViewModel(new StackOverflowFlair()) },
                    { "TwitterTimeline", new TwitterTimeline() },
                    { "ArchiveLinks", Posts.MonthlyArchiveLinks },
                };
            }
        } 

        public SiteViewModel(HttpContextWrapper httpContext) {
            HttpContext = httpContext;
        }

        protected readonly HttpContextWrapper HttpContext;
        private Posts _posts;
    }
}
