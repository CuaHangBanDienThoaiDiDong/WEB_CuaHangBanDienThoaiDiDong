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
    
    public partial class Districts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Districts()
        {
            this.Wards = new HashSet<Wards>();
            this.tb_Store = new HashSet<tb_Store>();
        }
    
        public int idDistricts { get; set; }
        public string name { get; set; }
        public Nullable<int> idProvinces { get; set; }
    
        public virtual Provinces Provinces { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Wards> Wards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_Store> tb_Store { get; set; }
    }
}