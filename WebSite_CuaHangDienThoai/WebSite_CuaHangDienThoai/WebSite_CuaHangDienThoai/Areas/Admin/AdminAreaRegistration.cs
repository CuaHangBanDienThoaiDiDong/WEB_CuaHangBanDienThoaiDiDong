using System.Web.Mvc;

namespace WebSite_CuaHangDienThoai.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
       name: "Admin_ThemMoiDongSanPham_Product",
       url: "Them-dong-san-pham-moi",
       defaults: new { controller = "Products", action = "Add" }
   );

            context.MapRoute(
       name: "Admin_DanhSachTaikhoankhachHang",
       url: "tai-khoan-khach-hang",
       defaults: new { controller = "AccountClient", action = "Index" }
   );


            context.MapRoute(
       name: "Admin_DangNhap",
       url: "he-thong-nhan-vien",
       defaults: new { controller = "Account", action = "DangNhap" }
   );

            context.MapRoute(
       name: "Admin_TrangChu",
       url: "he-thong-LTD",
       defaults: new { controller = "HomePage", action = "index" }
   );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}