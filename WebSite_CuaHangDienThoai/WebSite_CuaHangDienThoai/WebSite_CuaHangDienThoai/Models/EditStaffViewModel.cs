using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class EditStaffViewModel
    {
        public int Id { get; set; } // StaffId
        public string Code { get; set; } // MSNV
        public string PhoneNumber { get; set; } // SDT
        public string NameStaff { get; set; } // TenNhanVien
        public string CitizenIdentificationCard { get; set; } // CCCD
        public string Email { get; set; } // Email
        public string Password { get; set; } // Password
        public DateTime? Birthday { get; set; } // Birthday
        public string Location { get; set; } // DiaChi
        public DateTime? DayofWork { get; set; } // NgayVaoLam
        public decimal Wage { get; set; } // Luong
        public string Sex { get; set; } // GioiTinh
        public string CreatedBy { get; set; } // CreatedBy
        public DateTime CreatedDate { get; set; } // CreatedDate
     
        public bool Clock { get; set; } // Clock
        public int? FunctionId { get; set; } // IDCHu chuc ngang
        public int? StoreId { get; set; } // StoreId
        public byte[] Image { get; set; } // Image
    }
}