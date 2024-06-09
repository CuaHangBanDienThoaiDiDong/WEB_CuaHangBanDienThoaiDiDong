using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebSite_CuaHangDienThoai.Models
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeSessionAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Kiểm tra xem Session["CustomerId"] hoặc có token hợp lệ không
            return httpContext.Session["CustomerId"] != null || ValidateToken(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Nếu phiên không hợp lệ, chuyển hướng đến trang đăng nhập
            filterContext.Result = new RedirectResult("~/Account/Login");
        }

        private bool ValidateToken(HttpContextBase httpContext)
        {
            var cookie = httpContext.Request.Cookies["access_token"];
            if (cookie != null)
            {
                var tokenString = cookie.Value;
                return AuthenticateToken(tokenString);
            }
            return false;
        }

        private bool AuthenticateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = WebConfigurationManager.AppSettings["JwtSecretKey"]; // Đọc key từ cấu hình
            var key = Encoding.ASCII.GetBytes(secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
