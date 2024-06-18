using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_TokenEditImportWareHouse
    {
        public List<Admin_TokenEditImportWareHouseItem> Items { get; set; }
        public int ImportWareHosue { get; set; }
        public int StaffId { get; set; }
        public int WarehouseId { get; set; }
        public int SupplierId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public tb_Staff Staff { get; set; }

    }
    public class Admin_TokenEditImportWareHouseItem
    {
        public int ImportWarehouseDetailId { get; set; }
        public int ImportWareHosueId { get; set; }
        public decimal QuanTity { get; set; }
        public int Quantity { get; set; }
        public int ProductDetailId { get; set; }
        public tb_ProductDetail Product { get; set; }

    }
}