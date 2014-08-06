using MarkdownBlog.Net.Web.App_Start;
using MarkdownBlog.Net.Web.Infrastructure;
using MarkdownBlog.Net.Web.Models;
using MarkdownBlog.Net.Web.Models.UserConfigPersistence;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace MarkdownBlog.Net.Web {
    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            IocContainer.RegisterDependencies();

            Application.Add("SiteSettings", new Site());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);
            BlogRouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_PostAuthenticateRequest(object s, EventArgs e) {
            if (!Context.Request.IsAuthenticated) return;
            Context.User = (new UserCollection()).GetUser(Context.User.Identity.Name);
        }
    }
}