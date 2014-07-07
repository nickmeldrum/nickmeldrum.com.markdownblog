using System.Web;
using MarkdownBlog.Net.Web.Models;

namespace MarkdownBlog.Net.Web.Services {
    public class LinkService {
        public string CanonicalUrlFromRelative(string relativeUrl) {
            var canonicalUrl = "http://" + ((Site)HttpContext.Current.Application["SiteSettings"]).Domain + relativeUrl;
            canonicalUrl = canonicalUrl.ToLowerInvariant().Trim();
            return canonicalUrl.TrimEnd('/');
        }
    }
}