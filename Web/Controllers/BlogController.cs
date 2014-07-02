using MarkdownBlog.Net.Web.Models;
using System;
using System.IO;
using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Controllers {
    public class BlogController : ControllerBase {
        public ActionResult Index() {
            return View(PostsMetadata.Instance.List);
        }

        public ActionResult Post(string postName) {
            try {
                return string.IsNullOrWhiteSpace(postName)
                    ? View("Index")
                    : View("Post", new Post(postName));
            }
            catch (FileNotFoundException ex) {
                HttpContextWrapper.SendHttpStatusResponse(404);
            }

            return View("Index");
        }

        public ActionResult Archive(string month, int year) {
            return View(new Archive() { Month = month, Year = year });
        }

        public ActionResult Feed(string type) {
            var feedType = FeedType.unknown;

            if (!Enum.TryParse(type.ToLower(), out feedType)) {
                HttpContextWrapper.SendHttpStatusResponse(404);
            }

            var feed = new Feed(PostsMetadata.Instance, HttpContextWrapper);

            return feed.GetFeedXml(feedType);
        }
    }
}