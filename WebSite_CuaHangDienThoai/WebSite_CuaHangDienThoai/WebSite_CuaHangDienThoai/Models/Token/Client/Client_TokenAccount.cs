using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models.Token.Client
{
    public class Client_TokenAccount
    {
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public string TenKhachHang { get; set; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public int  SDT { get; set; }


        public string token { get; set; }


    }
}