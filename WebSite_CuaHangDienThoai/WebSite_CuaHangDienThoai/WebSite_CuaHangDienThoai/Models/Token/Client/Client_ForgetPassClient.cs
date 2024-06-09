using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models.Token.Client
{
    public class Client_ForgetPassClient
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Nhập mã từ email.")]
        public string Code { get; set; }
        public string token { get; set; }
    }
}