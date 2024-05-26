using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebSite_CuaHangDienThoai.Models
{
    public class EditVoucherViewModel
    {

        public int VoucherId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        public string Title { get; set; }

        public string CreatedBy { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã voucher.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng nhập số lượng hợp lệ.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phần trăm giảm giá.")]
        [Range(0, 100, ErrorMessage = "Vui lòng nhập số từ 0 đến 100.")]
        public int PhanTramGiaGiam { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu.")]
        public DateTime? UsedDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc.")]
        public DateTime? ModifiedDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}