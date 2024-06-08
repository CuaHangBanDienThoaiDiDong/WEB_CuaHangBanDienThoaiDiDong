using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class CLient_EditCustomerViewModel
    {



        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        public string PhoneNumber { get; set; }

        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã voucher.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã voucher.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc.")]
        public DateTime? Birthday { get; set; }

        public string Loaction { get; set; }

        public int NumberofPurchases { get; set; }

        public string Code { get; set; }
        public bool Clock { get; set; }

        public byte[] Image { get; set; } 




      
    }
}