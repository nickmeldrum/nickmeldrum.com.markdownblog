﻿using MarkdownBlog.Net.Web.NavigationRoutes;
using System.Web.Routing;

namespace MarkdownBlog.Net.Web.App_Start
{
    public class BlogRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapNavigationRoute("Blog-navigation", "Blog", "blog", new { controller = "Blog", action = "Index" });
        }
    }
}
