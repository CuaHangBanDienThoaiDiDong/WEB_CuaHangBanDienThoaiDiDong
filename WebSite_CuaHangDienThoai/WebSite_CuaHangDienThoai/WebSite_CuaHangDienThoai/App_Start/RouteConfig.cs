using System.Web.Mvc;
using System.Web.Routing;

namespace WebSite_CuaHangDienThoai
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //// Route Chi tiet san pham
            routes.MapRoute(
             name: "detailProduct",
             url: "chi-tiet/{alias}-p{id}",
             defaults: new { controller = "Product", action = "Details", alias = UrlParameter.Optional },
             namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
         );
            //// Route cho trang khoi phuc mat khau
            routes.MapRoute(
                name: "Forgotpassword",
                url: "khoi-phuc-mat-khau",
                defaults: new { controller = "Account", action = "Forgotpassword", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );
            //// Route cho trang dang ky
            routes.MapRoute(
                name: "Register",
                url: "dang-ky",
                defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );
            //// Route cho trang login
            routes.MapRoute(
                name: "Login",
                url: "dang-nhap",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );
            //// Route cho trang chi tiết danh mục sản phẩm
            routes.MapRoute(
                name: "CategoryProduct",
                url: "danh-muc-san-pham/{alias}-{id}",
                defaults: new { controller = "ProductCategory", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );

            // Route cho trang danh sách sản phẩm
            routes.MapRoute(
                name: "ProductsIndex",
                url: "san-pham",
                defaults: new { controller = "Product", action = "Index" },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );

            // Route mặc định
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );

            // Route cho trang lỗi 404
            routes.MapRoute(
                name: "404-PageNotFound",
                url: "Loi-he-thong-may-chu",
                defaults: new { controller = "Error", action = "NotFound" }
            );

            // Route cho trang lỗi 500
            routes.MapRoute(
                name: "500-InternalServerError",
                url: "Loi-he-thong-may-chu-500-server",
                defaults: new { controller = "Error", action = "ServerError" }
            );
        }
    }
}
