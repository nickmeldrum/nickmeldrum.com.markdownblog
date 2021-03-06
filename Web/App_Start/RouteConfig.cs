﻿using MarkdownBlog.Net.Web.Models;
using System.Web.Mvc;
using System.Web.Routing;

namespace MarkdownBlog.Net.Web.App_Start {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Specific post and page routes:

            routes.MapRoute(
                "BlogFeed", // Route name
                "blog/feed/{*type}", // URL with parameters
                new { controller = "Blog", action = "Feed", type = "Atom"  } // Parameter defaults
            );

            routes.MapRoute(
                "BlogArchiveItem", // Route name
                "blog/archive/{month}/{year}", // URL with parameters
                new { controller = "Archive", action = "Item" } // Parameter defaults
            );

            routes.MapRoute(
                "BlogArchiveIndex", // Route name
                "blog/archive", // URL with parameters
                new { controller = "Archive", action = "Index" } // Parameter defaults
            );

            routes.MapRoute(
                "BlogPost", // Route name
                "blog/{postName}", // URL with parameters
                new { controller = "Blog", action = "Post" } // Parameter defaults
            );

            foreach (var pageMetadata in PagesMetadata.Instance.List) {
                routes.MapRoute(
                    "Page" + pageMetadata.Slug, // Route name
                    pageMetadata.Slug, // URL with parameters
                    new { controller = "Page", action = "GetPage", pageName = pageMetadata.Slug } // Parameter defaults
                );
            }

            routes.MapRoute(
                "Sitemap", // Route name
                "sitemap", // URL with parameters
                new { controller = "Home", action = "Sitemap" } // Parameter defaults
            );

            routes.MapRoute(
                "DoNotTrack", // Route name
                "donottrack", // URL with parameters
                new { controller = "Home", action = "DoNotTrack" }, // Parameter defaults
                new[] { "MarkdownBlog.Net.Web.Controllers" }
            );

            // Default Route:

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{*id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new[] { "MarkdownBlog.Net.Web.Controllers" }
            );
        }
    }
}