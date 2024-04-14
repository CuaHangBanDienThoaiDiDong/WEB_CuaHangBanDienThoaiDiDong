using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_TokenProductDetail
    {
        public string Color { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSale { get; set; }

        public decimal OrigianlPrice { get; set; }
        public bool TypeProduct { get; set; }
        public int DungLuongPin { get; set; }
        public int Ram  {get; set; }
        public int DungLuong  {get; set; }
        public int ProductsId { get; set; }


    }
}