//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChingChing.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BILLDETAIL
    {
        public int IDBILLDETAIL { get; set; }
        public Nullable<int> IDBILL { get; set; }
        public Nullable<int> IDPRO { get; set; }
        public Nullable<short> QUANTITY { get; set; }
    
        public virtual BILL BILL { get; set; }
        public virtual PRODUCT PRODUCT { get; set; }
    }
}
