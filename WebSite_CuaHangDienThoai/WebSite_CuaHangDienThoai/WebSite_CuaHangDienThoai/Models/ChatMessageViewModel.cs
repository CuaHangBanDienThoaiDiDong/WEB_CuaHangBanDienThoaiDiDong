using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class ChatMessageViewModel
    {
        public int? DetailId { get; set; }
        public int? MessageId { get; set; }
        public int? CustomerId { get; set; }
        public int? StaffId { get; set; }
        public string MessageContent { get; set; }
        public DateTime Time { get; set; }
        public DateTime Timestamp { get; set; }
        public bool? IsRead { get; set; }
        public string StaffMessageContent { get; set; }
        public DateTime? StaffMessageTimestamp { get; set; }
        public string CustomerName { get; set; } // Tên khách hàng
        public string StaffName { get; set; } // Tên nhân viên
        public byte[] CustomerImage { get; set; } // Ảnh khách hàng
        public byte[] StaffImage { get; set; } // Ảnh nhân viên
    }
}
