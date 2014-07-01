using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MarkdownBlog.Net.Web.NavigationRoutes;

namespace MarkdownBlog.Net.Web.App_Start {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "BlogFeed", // Route name
                "blog/feed/{*type}", // URL with parameters
                new { controller = "Blog", action = "Feed", type = "Atom"  } // Parameter defaults
            );

            routes.MapRoute(
                "BlogArchive", // Route name
                "blog/archive/{month}/{year}", // URL with parameters
                new { controller = "Blog", action = "Archive" } // Parameter defaults
            );

            routes.MapRoute(
                "BlogPost", // Route name
                "blog/{postName}", // URL with parameters
                new { controller = "Blog", action = "Post" } // Parameter defaults
            );

            var pagesDir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Pages/"));
            foreach (var file in pagesDir.GetFiles("*.md")) {
                var fileName = Path.GetFileNameWithoutExtension(file.Name);
                routes.MapRoute(
                    "Page" + fileName, // Route name
                    fileName, // URL with parameters
                    new { controller = "Page", action = "GetPage", pageName = fileName } // Parameter defaults
                );
            }

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{*id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }
    }
}