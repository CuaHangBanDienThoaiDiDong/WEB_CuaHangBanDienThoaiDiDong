using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class EditStaffViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public string NameStaff { get; set; }
        public string CitizenIdentificationCard { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public DateTime? Birthday { get; set; }
        public string Location { get; set; }
        public DateTime? DayofWork { get; set; }
        public decimal Wage { get; set; }
        public string Sex { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
   
   
        public bool Clock { get; set; }
        public int FunctionId { get; set; }
        public int StoreId { get; set; }
        public byte[] Image { get; set; }
    }
}