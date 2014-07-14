using System.Web.Mvc;
using MarkdownBlog.Net.Web.Models;

namespace MarkdownBlog.Net.Web.Controllers {
    public class ArchiveController : ControllerBase {

        public ActionResult Index() {
            return View(PostsMetadata.Instance.MonthlyArchiveLinks);
        }

        public ActionResult Item(string month, int year) {
            return View(new Archive() { Month = month, Year = year });
        }
    }
}