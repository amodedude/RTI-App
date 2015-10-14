// -----------------------------------------------------------------------
// <copyright file="MvcApplication.cs" company="RTI">
// RTI
// </copyright>
// <summary>Mvc Application</summary>
// -----------------------------------------------------------------------

using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RTI.ModelingSystem.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filter)
        {
            filter.Add(new HandleErrorAttribute());
        }
    }
}
