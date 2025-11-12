using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShoeStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ProductDetail",
                url: "products/{slug}/{styleColor}",
                defaults: new { controller = "Products", action = "Details" },
                namespaces: new[] { "ShoeStore.Controllers" }
            );

            routes.MapRoute(
                name: "ProductDetailNoColor",
                url: "products/{slug}",
                defaults: new { controller = "Products", action = "Details", styleColor = UrlParameter.Optional },
               namespaces: new[] { "ShoeStore.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ShoeStore.Controllers" }
            );
        }
    }
}
