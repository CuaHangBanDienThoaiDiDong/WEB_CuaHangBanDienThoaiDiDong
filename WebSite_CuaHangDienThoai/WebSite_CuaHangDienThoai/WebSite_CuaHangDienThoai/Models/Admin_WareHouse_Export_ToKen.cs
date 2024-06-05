using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_WareHouse_Export_ToKen
    {
        [Required(ErrorMessage = "Không để trống mã code ")]
        public string Code { get; set; }
        //[Required(ErrorMessage = "Không để trống kho")]
        //public int Idkho { get; set; }
    }
}