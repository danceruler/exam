using FluentScheduler;
using FP_wab.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FP_wab
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Registry registry = new Registry();
            registry.Schedule<AutoUpdateExamTime>().NonReentrant().ToRunNow().AndEvery(60).Seconds();
            JobManager.Initialize(registry);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
