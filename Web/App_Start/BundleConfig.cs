using System.Web.Optimization;

namespace MarkdownBlog.Net.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js").Include(
                "~/scripts/jquery-{version}.js",
                "~/scripts/bootstrap-{version}.js",
                "~/scripts/jquery.validate-1.13.0.js",
                "~/scripts/prettify.js",
                "~/scripts/runprettify.js",
                "~/scripts/search.js"
                ));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/content/bootstrap.css",
                "~/content/body.css",
                "~/content/prettify.css",
                "~/content/sunburst-modified.css",
                "~/content/bootstrap-theme.css"
                ));
        }
    }
}