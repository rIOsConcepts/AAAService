//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AAAService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Phone_Numbers
    {
        public System.Guid guid { get; set; }
        public System.Guid owner_guid { get; set; }
        public string phone_num { get; set; }
        public int phone_type { get; set; }
        public bool active { get; set; }
        public System.DateTime last_modified { get; set; }
    }
}
