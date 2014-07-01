using System.IO;
using System.Web;
using System.Web.Mvc;
using MarkdownBlog.Net.Web.NavigationRoutes;
using System.Web.Routing;

namespace MarkdownBlog.Net.Web.App_Start
{
    public class BlogRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("Glimpse.axd");

            routes.MapNavigationRoute("Blog-navigation", "Blog", "blog", new { controller = "Blog", action = "Index" });

                var pagesDir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Pages/"));
                foreach (var file in pagesDir.GetFiles("*.md"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.Name);
                    var displayName = fileName.Replace('-', ' ');
                    routes.MapNavigationRoute("Page-navigation-" + fileName, displayName, fileName, new { controller = "Page", action = "GetPage" });
                }
        }
    }
}
