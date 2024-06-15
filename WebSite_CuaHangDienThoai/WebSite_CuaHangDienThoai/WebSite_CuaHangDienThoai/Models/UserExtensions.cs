using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Web;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Helpers
{
    public static class UserExtensions
    {
        public static ApplicationUser GetCurrentApplicationUser(this HttpContext httpContext, UserManager<ApplicationUser> userManager)
        {
            var userId = httpContext.User.Identity.GetUserId();
            if (userId != null)
            {
                return userManager.FindByIdAsync(userId).Result;
            }
            return null;
        }
    }
}
