using System.Web;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Helpers
{
    public static class UserExtensions
    {
        public static object GetCurrentUserInfo(this HttpContextBase httpContext)
        {
            if (httpContext.Session["CustomerId"] != null)
            {
                // Đối với khách hàng, bạn có thể trả về một đối tượng chứa thông tin khách hàng từ session
                return null; // Bạn cần thay thế dòng này để trả về thông tin khách hàng
            }
            else if (httpContext.Session["user"] != null)
            {
                // Đối với admin, bạn có thể trả về một đối tượng chứa thông tin admin từ session
                return httpContext.Session["user"]; // Đây là một ví dụ, bạn cần điều chỉnh dòng này để trả về thông tin admin
            }
            else
            {
                // Không có ai đăng nhập
                return null;
            }
        }
    }
}
