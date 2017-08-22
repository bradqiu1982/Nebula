using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nebula
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Nebula",
                url: "Nebula/{controller}/{action}",
                defaults: new { controller = "MiniPIP", action = "ViewAll" }
            );

            //WHEN REAL DEPLOY , THIS SHOULD BE COMMENTED
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}",
            //    defaults: new { controller = "MiniPIP", action = "ViewAll" }
            //);

        }
    }
}
