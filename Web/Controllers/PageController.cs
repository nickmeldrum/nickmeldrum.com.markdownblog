using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Controllers
{
    public class PageController : Controller
    {
        public ContentResult GetPage()
        {
            return new ContentResult() { Content = "oh hai"  };
        }
    }
}
