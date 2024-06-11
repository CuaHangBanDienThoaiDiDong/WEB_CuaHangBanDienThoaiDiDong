using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
namespace WebSite_CuaHangDienThoai
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình xác thực và ủy quyền ở đây
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/DangNhap"), // Đường dẫn đến trang đăng nhập
                LogoutPath = new PathString("/Account/DangXuat"), // Đường dẫn đến trang đăng xuất
                ExpireTimeSpan = TimeSpan.FromMinutes(30) // Thời gian hết hạn phiên làm việc
            });
        }
    }
}