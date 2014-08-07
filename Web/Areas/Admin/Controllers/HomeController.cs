using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Areas.Admin.Controllers {
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller {
        public ActionResult Index() {
            return View();
        }
    }
}
