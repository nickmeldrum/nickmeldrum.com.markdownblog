namespace MarkdownBlog.Net.Web {
    using MarkdownBlog.Net.Web.Infrastructure;
    using System.Web.Optimization;

    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            var jsBundle = new ScriptBundle("~/js").Include(
                "~/scripts/jquery-{version}.js",
                "~/scripts/bootstrap-{version}.js",
                "~/scripts/jquery.validate-1.13.0.js",
                "~/scripts/jquery-validate-bootstrap3-overrides.js",
                "~/scripts/syntax.js",
                "~/scripts/search.js"
                );
            bundles.Add(jsBundle);

            var desktopOnlyJsBundle = new ScriptBundle("~/desktopjs").Include(
                "~/scripts/skrollr.js",
                "~/scripts/visuals.js"
            );
            bundles.Add(desktopOnlyJsBundle);

            var lessBundle = new Bundle("~/css").Include(
                "~/content/bootstrap.css",
                "~/content/body.less",
                "~/content/bootstrap-theme.css"
            );

            lessBundle.Transforms.Add(new LessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

        }
    }
}
