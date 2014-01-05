using System.Configuration;

namespace MarkdownBlog.Net.Web.Models {
    public class Site {
        public string Name { get { return ConfigurationManager.AppSettings["Name"] ?? "Default Site Name"; } }
        public string Description { get { return ConfigurationManager.AppSettings["Description"] ?? "Default Site Description"; } }
        public string Owner { get { return ConfigurationManager.AppSettings["Owner"] ?? "Default Site Owner Name"; } }
        public string DisqusShortName { get { return ConfigurationManager.AppSettings["DisqusShortName"] ?? "DefaultDisqusShortName"; } }
    }
}