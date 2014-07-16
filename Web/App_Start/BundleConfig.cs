using System.Web.Optimization;
using MarkdownBlog.Net.Web.Infrastructure;

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
                "~/scripts/jquery-validate-bootstrap3-overrides.js",
                "~/scripts/prettify.js",
                "~/scripts/skrollr.js",
                "~/scripts/runprettify.js",
                "~/scripts/search.js",
                "~/scripts/visuals.js"
                ));

            var lessBundle = new Bundle("~/content/css")
                .Include(
                    "~/content/bootstrap.css",
                    "~/content/body.less",
                    "~/content/prettify.css",
                    "~/content/sunburst-modified.css",
                    "~/content/bootstrap-theme.css"
                )
                .IncludeDirectory("~/Content", "*.less");

            lessBundle.Transforms.Add(new LessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

        }
    }
}