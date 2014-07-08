using System.Configuration;

namespace MarkdownBlog.Net.Web.Models {
    public class Site {
        public string Name { get { return ConfigurationManager.AppSettings["Name"] ?? "Default Site Name"; } }
        public string Domain { get { return ConfigurationManager.AppSettings["Domain"] ?? "DefaultDomain.com"; } }
        public string Description { get { return ConfigurationManager.AppSettings["Description"] ?? "Default Site Description"; } }
        public string Owner { get { return ConfigurationManager.AppSettings["Owner"] ?? "Default Site Owner Name"; } }
        public string DisqusShortName { get { return ConfigurationManager.AppSettings["DisqusShortName"] ?? "DefaultDisqusShortName"; } }
        public string GoogleTagManagerAccount { get { return ConfigurationManager.AppSettings["GoogleTagManagerAccount"] ?? "DefaultGoogleTagManagerAccount"; } }
        public bool ShowDrafts { get { return bool.Parse(ConfigurationManager.AppSettings["ShowDrafts"]); } }
    }
}