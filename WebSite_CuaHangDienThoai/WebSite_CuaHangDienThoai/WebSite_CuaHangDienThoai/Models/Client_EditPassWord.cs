using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Client_EditPassWord
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string Password { get; set; }
        public string PasswordNew { get; set; }

      
    }
}