using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_TokenStore
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public bool isStatus { get; set; }
        public int idProvinces { get; set; }
        public int idDistricts { get; set; }
        public int idWards { get; set; }
    }
}