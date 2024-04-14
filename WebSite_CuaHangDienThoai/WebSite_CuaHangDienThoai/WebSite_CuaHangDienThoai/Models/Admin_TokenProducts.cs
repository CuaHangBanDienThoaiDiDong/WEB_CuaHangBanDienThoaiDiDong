using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_TokenProducts
    {
        public string Title { get; set; }

        public string Image { get; set; }
        
        //public int Quantity { get; set; }
        public int ViewCount { get; set; }
        public bool IsHome { get; set; }
        public bool IsSale { get; set; }
        public bool IsFeature { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }

      
  
    
        public string HeDieuHanh { get; set; }
       
        public string TocDoGPU { get; set; }
        public string TocDoCPU { get; set; }




        public string MangDiDong { get; set; }
        public string Sim { get; set; }
        public string Wifi { get; set; }
        public string GPS { get; set; }

        public string Bluetooth { get; set; }
        public string CongKetNoi { get; set; }
        public string JackTaiNghe { get; set; }

        public string LoaiPin { get; set; }
        public string HoTroSac { get; set; }
        public string CongNghePin { get; set; }


        public string CPU { get; set; }
        public string GPU { get; set; }
        public float ManHinh { get; set; }

        public int ProductCategoryId { get; set; }
        public int ProductCompanyId { get; set; }


    }
}