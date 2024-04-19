using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models.Token.Client
{
    public class Client_ForgetPassClient
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
    }
}