using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class ProductDetailViewModel
    {
        public int Ram { get; set; }
        public int DungLuong { get; set; }
        public string Color { get; set; }
        public int ProductDetailId { get; set; }
       


        public decimal Price { get; set; }
        public decimal PriceSale { get; set; }
        public decimal OrigianlPrice { get; set; }
        public int ProductsId { get; set; }

    }
}