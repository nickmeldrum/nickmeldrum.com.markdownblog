namespace MarkdownBlog.Net.Web.Models {
    public class GithubBadgeViewModel {
        private readonly GithubBadge _data;

        public GithubBadgeViewModel(GithubBadge data) {
            _data = data;
        }

        public string BadgeSource {
            get {
                return string.Format("http://githubbadge.appspot.com/{0}", _data.UserName);
             }
        }
    }
}
