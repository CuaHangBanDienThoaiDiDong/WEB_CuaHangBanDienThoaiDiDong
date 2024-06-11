using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class Admin_TokenEditBill
    {
        public List<Admin_TokenEditBillItem> Items { get; set; }
        public int SellerId { get; set; }
        public string Code { get; set; }
        public int CustomerId { get; set; }
        public string Phone { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public int TypePayment { get; set; }
        public int StaffId { get; set; }
    }
    public class Admin_TokenEditBillItem 
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ProductDetailId { get; set; }
    }
}