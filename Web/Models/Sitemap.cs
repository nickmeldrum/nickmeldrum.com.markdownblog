using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MarkdownBlog.Net.Web.Services;

namespace MarkdownBlog.Net.Web.Models
{
    public class Sitemap
    {
        private readonly LinkService _linkService = new LinkService();

        public IEnumerable Links
        {
            get
            {
                var links = new List<string> { "/", "/blog", "/blog/feed", "/search", "/sitemap" };

                links.AddRange(PagesMetadata.Instance.List.Select(page => "/" + page.Slug));

                links.AddRange(PostsMetadata.Instance.List.Select(post => "/blog/" + post.Slug));

                return links.Select(url => _linkService.CanonicalUrlFromRelative(url));
            }
        }
    }
}