using System;
using System.Web;
using System.Web.Mvc;
using MarkdownBlog.Net.Web.Models;

namespace MarkdownBlog.Net.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View(PostsMetadata.Instance.Latest(3));
        }

        public ActionResult Sitemap()
        {
            Response.ContentType = "text/xml";
            return View(new Sitemap());
        }

        public ActionResult DoNotTrack()
        {
            var doNotTrackCookie = new HttpCookie("DoNotTrack") {Expires = DateTime.Now.AddYears(10)};
            doNotTrackCookie.Values.Add("DoNotTrack", "DoNotTrack");

            Response.Cookies.Add(doNotTrackCookie);

            return View();
        }
    }
}
