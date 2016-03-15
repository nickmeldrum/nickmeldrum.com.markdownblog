using System.Configuration;

namespace MarkdownBlog.Net.Web.Models {
    public class GithubBadge {
        public GithubBadge()
        {
            UserName = ConfigurationManager.AppSettings["GithubBadgeUserName"] ?? "DefaultGithubBadgeUserName";
        }

        public string UserName { get; private set; }
    }
}
