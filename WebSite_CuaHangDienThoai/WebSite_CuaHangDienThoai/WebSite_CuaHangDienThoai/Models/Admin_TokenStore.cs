using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_TokenStore
    {
        [Required(ErrorMessage = "Vui lòng nhập tên cửa hàng.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        public string Location { get; set; }

        public bool isStatus { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố.")]
        public int idProvinces { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn quận/huyện.")]
        public int idDistricts { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn phường/xã.")]
        public int idWards { get; set; }
    }
}