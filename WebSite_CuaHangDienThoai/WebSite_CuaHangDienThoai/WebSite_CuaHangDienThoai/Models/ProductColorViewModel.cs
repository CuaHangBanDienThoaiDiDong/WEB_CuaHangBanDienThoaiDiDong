using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class ProductColorViewModel
    {
        public int ProductDetailId { get; set; }
        public string Color { get; set; }
        public int ProductslId { get; set; }

        public int Capacity { get; set; }
        public byte[]  Image { get; set; }
    }
}