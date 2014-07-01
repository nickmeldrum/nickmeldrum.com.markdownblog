using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Controllers
{
    public class PageController : Controller
    {
        public ContentResult GetPage(string pageName)
        {
            return new ContentResult() { Content = "oh hai" + pageName  };
        }
    }
}
