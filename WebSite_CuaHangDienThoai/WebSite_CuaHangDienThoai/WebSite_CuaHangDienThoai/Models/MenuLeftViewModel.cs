using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class MenuLeftViewModel
    {
        public List<tb_ProductCategory> AllCategories { get; set; }
        public List<tb_Products> Accessories { get; set; }
    }
}