//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CINEMA_BE
{
    using System;
    using System.Collections.Generic;
    
    public partial class voucher_uses
    {
        public int id { get; set; }
        public Nullable<int> id_customer { get; set; }
        public Nullable<int> id_voucher { get; set; }
        public Nullable<System.DateTime> date_used { get; set; }
    
        public virtual customer customer { get; set; }
        public virtual voucher voucher { get; set; }
    }
}
