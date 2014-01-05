using System.Configuration;

namespace MarkdownBlog.Net.Web.Models {
    public class StackOverflowFlair {
        public StackOverflowFlair()
        {
            Id = int.Parse(ConfigurationManager.AppSettings["StackOverflowFlairId"] ?? "-1");
            UserName = ConfigurationManager.AppSettings["StackOverflowFlairUserName"] ?? "DefaultStackOverflowUserName";
            DisplayName = ConfigurationManager.AppSettings["StackOverflowFlairDisplayName"] ?? "Default Stack Overflow Display Name";
            UseStackExchangeFlair = bool.Parse(ConfigurationManager.AppSettings["StackOverflowFlairUseStackExchangeFlair"] ?? "False");
            Theme = ConfigurationManager.AppSettings["StackOverflowFlairTheme"] ?? "default";
        }

        public int Id { get; private set; }
        public string UserName { get; private set; }
        public string DisplayName { get; private set; }
        public bool UseStackExchangeFlair { get; private set; }
        public string Theme { get; private set; }
    }
}