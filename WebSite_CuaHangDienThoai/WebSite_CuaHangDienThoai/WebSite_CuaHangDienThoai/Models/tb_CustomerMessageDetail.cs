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
    
    public partial class tb_CustomerMessageDetail
    {
        public int DetailId { get; set; }
        public int MessageId { get; set; }
        public int CustomerId { get; set; }
        public string Content { get; set; }
        public System.DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    
        public virtual tb_Customer tb_Customer { get; set; }
        public virtual tb_Message tb_Message { get; set; }
    }
}
