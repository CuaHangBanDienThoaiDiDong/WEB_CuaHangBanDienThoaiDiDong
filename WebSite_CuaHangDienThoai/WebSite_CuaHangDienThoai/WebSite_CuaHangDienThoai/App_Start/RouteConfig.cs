using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebSite_CuaHangDienThoai
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );


            routes.MapRoute(
                  name: "CategoryProduct",
                  url: "danh-muc-san-pham/{alias}-{id}",
                  defaults: new { controller = "ProductCategory", action = "Detail", id = UrlParameter.Optional },
                  namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
              );
        }
    }
}
