using System.Web.Optimization;

namespace GestionColegios
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.7.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                    "~/Scripts/kendo/kendo.all.min.js",
                    "~/Scripts/kendo/cultures/kendo.culture.es-CL.min.js"));

            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
                    "~/Content/kendo/kendo.default-main.css"));
        }
    }
}
