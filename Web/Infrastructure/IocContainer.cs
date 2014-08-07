using Autofac;
using Autofac.Integration.Mvc;
using MarkdownBlog.Net.Web.Models.UserConfigPersistence;
using MarkdownBlog.Net.Web.Services;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Infrastructure {
    public class IocContainer {
        public static void RegisterDependencies() {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerRequest();

            builder.RegisterType<FormsIdentityService>().As<IFormsIdentityService>().InstancePerRequest();
            builder.RegisterType<FormsAuthenticationService>().As<IFormsAuthenticationService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<UserCollection>();
            builder.RegisterInstance(Thread.CurrentPrincipal.Identity).As<IIdentity>().SingleInstance();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}