using MarkdownBlog.Net.Web.App_Start;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MarkdownBlog.Net.Web.Models;

namespace MarkdownBlog.Net.Web {
    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            Application.Add("SiteSettings", new Site());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);
            BlogRouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}