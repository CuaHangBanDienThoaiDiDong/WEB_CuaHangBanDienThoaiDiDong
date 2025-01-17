﻿using System.Web.Mvc;
using System.Web.Routing;

namespace WebSite_CuaHangDienThoai
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
             name: "Voucher",
             url: "voucher",
             defaults: new { controller = "Voucher", action = "Index", id = UrlParameter.Optional },
             namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
         );
            routes.MapRoute(
             name: "DieuKien",
             url: "dieu-kien-voucher/p{id}",
             defaults: new { controller = "Voucher", action = "DieuKien", id = UrlParameter.Optional },
             namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
         );
            routes.MapRoute(
               name: "TinNhan",
               url: "tin-nhan/p{id}",
               defaults: new { controller = "Chat", action = "Mess", id = UrlParameter.Optional },
               namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
           );
            //// Huy don hang thnah cong
            routes.MapRoute(
             name: "HuyDonHangThanhCong",
             url: "huy-don-hang-thanh-cong/{code}-p{id}",
             defaults: new { controller = "Order", action = "SuccessCancelOrder", alias = UrlParameter.Optional },
             namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
         );

            //// Don hang
            routes.MapRoute(
                name: "DonHangKhachHang",
                url: "don-hang",
                defaults: new { controller = "Order", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );


            //// Thonmg tin khách hang
            routes.MapRoute(
                name: "Muahnangthnahcong",
                url: "mua-hang-thanh-cong",
                defaults: new { controller = "ShoppingCart", action = "CheckOutSucces", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );
            //// Thonmg tin khách hang
            routes.MapRoute(
                name: "Taikhoankhachhang",
                url: "tai-khoan/p{id}",
                defaults: new { controller = "Account", action = "Profile", id = UrlParameter.Optional },
                namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
            );
            //// Route Mua hang thanh cong
            routes.MapRoute(
       name: "Muahhang",
       url: "mua-hang-thanh-cong",
       defaults: new { controller = "ShoppingCart", action = "CheckOutSucces", alias = UrlParameter.Optional },
       namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
   );
            //// Route Thanh toan
            routes.MapRoute(
       name: "ThanhTasn",
       url: "thanh-toan",
       defaults: new { controller = "ShoppingCart", action = "CheckOut", alias = UrlParameter.Optional },
       namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
   );

            //// Route Chi tiet san pham
            routes.MapRoute(
       name: "LoaSanphamtheoCongTy", 
       url: "dong-san-pham/{TitleCategory}-{TitleCompany}-{CompanyId}-{CategoryId}",
       defaults: new { controller = "Product", action = "ProductsByCompany", alias = UrlParameter.Optional },
       namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
   );



            routes.MapRoute(
                 name: "Search",
                 url: "tim-kiem/key={key}",
                 defaults: new { controller = "Product", action = "Search" }
             );

            //// Route sanpham-dungluong
            routes.MapRoute(
             name: "ProductDetailByCapacity",
             url: "chi-tiet-dungluong/{Productsid}-DungLuong{Capacity}",
             defaults: new { controller = "ProductDetail", action = "ProductDetailByCapacity", alias = UrlParameter.Optional },
             namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
         );
            //// Route Gio-hang
            routes.MapRoute(
             name: "indexCart",
             url: "Gio-hang",
             defaults: new { controller = "ShoppingCart", action = "Index", alias = UrlParameter.Optional },
             namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
         );
            //// Route Trang chu
            routes.MapRoute(
             name: "indexHome",
             url: "trang-chu",
             defaults: new { controller = "Home", action = "Index", alias = UrlParameter.Optional },
             namespaces: new[] { "WebSite_CuaHangDienThoai.Controllers" }
         );
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
