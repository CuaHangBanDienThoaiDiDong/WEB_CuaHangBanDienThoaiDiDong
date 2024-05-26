using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_TokenVoucher
    {
      
        public string Title { get; set; }
        public int Quantity { get; set; }
        public int PercentPriceReduction { get; set; }

        public DateTime UsedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}