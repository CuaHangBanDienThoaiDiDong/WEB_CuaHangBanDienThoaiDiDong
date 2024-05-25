using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_TokenProducts
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        public string Title { get; set; }

        public string Image { get; set; }

        //public int Quantity { get; set; }
        public int ViewCount { get; set; }
        public bool IsHome { get; set; }
        public bool IsSale { get; set; }
        public bool IsFeature { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }



        [Required(ErrorMessage = "Hệ điều là bắt buộc.")]
        public string HeDieuHanh { get; set; }

        [Required(ErrorMessage = "Tốc độ GPU là bắt buộc.")]
        public string TocDoGPU { get; set; }

        [Required(ErrorMessage = "Tốc độ CPU là bắt buộc.")]
        public string TocDoCPU { get; set; }




        [Required(ErrorMessage = "Mang di động là bắt buộc.")]
        public string MangDiDong { get; set; }

        [Required(ErrorMessage = "Sim là bắt buộc.")]
        public string Sim { get; set; }

        [Required(ErrorMessage = "Wifi là bắt buộc.")]
        public string Wifi { get; set; }
        [Required(ErrorMessage = "GPS là bắt buộc.")]
        public string GPS { get; set; }
        [Required(ErrorMessage = "Bluetooth là bắt buộc.")]
        public string Bluetooth { get; set; }
        [Required(ErrorMessage = "Cổng kết là bắt buộc.")]
        public string CongKetNoi { get; set; }
        [Required(ErrorMessage = "Jack tai nghe là bắt buộc.")]
        public string JackTaiNghe { get; set; }
        [Required(ErrorMessage = "Loại pin là bắt buộc.")]
        public string LoaiPin { get; set; }
        [Required(ErrorMessage = "Hỗ trợ sạc là bắt buộc.")]
        public string HoTroSac { get; set; }
        [Required(ErrorMessage = "Công nghệ pin là bắt buộc.")]
        public string CongNghePin { get; set; }

        [Required(ErrorMessage = "CPU là bắt buộc.")]
        public string CPU { get; set; }
        [Required(ErrorMessage = "GPU là bắt buộc.")]
        public string GPU { get; set; }
        [Required(ErrorMessage = "Tỷ lệ màn hình là bắt buộc.")]
        public float ManHinh { get; set; }
        [Required(ErrorMessage = "Dung lượng pin là bắt buộc.")]
        public int DungLuongPin { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductCompanyId { get; set; }


    }
}