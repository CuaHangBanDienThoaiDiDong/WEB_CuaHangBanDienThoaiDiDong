//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebSite_CuaHangDienThoai.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_ImportWarehouseDetail
    {
        public int ImportWarehouseDetailId { get; set; }
        public Nullable<int> ProductDetailId { get; set; }
        public Nullable<int> QuanTity { get; set; }
        public Nullable<int> ImportWarehouseId { get; set; }
    
        public virtual tb_ImportWarehouse tb_ImportWarehouse { get; set; }
    }
}