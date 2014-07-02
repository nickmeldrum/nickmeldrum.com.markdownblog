using System.Collections.Generic;
using System.Web.Mvc;
using MarkdownBlog.Net.Web.Models;

namespace MarkdownBlog.Net.Web.Controllers
{
    public class AsidesController : Controller
    {
        [ChildActionOnly]
        public PartialViewResult Index()
        {
            return PartialView(new Dictionary<string, object>
                            {
                                {"StackOverflowFlair", new StackOverflowFlairViewModel(new StackOverflowFlair())},
                                {"TwitterTimeline", new TwitterTimeline()},
                                {"ArchiveLinks", PostsMetadata.Instance.MonthlyArchiveLinks}
                            });
        }
    }
}