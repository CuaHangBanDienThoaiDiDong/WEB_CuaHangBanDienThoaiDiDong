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
    
    public partial class tb_Voucher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_Voucher()
        {
            this.tb_VoucherDetail = new HashSet<tb_VoucherDetail>();
        }
    
        public int VoucherId { get; set; }
        public string Code { get; set; }
        public Nullable<int> PercentPriceReduction { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Modifeby { get; set; }
        public string UsedBy { get; set; }
        public Nullable<System.DateTime> UsedDate { get; set; }
        public Nullable<int> Quantity { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_VoucherDetail> tb_VoucherDetail { get; set; }
    }
}
