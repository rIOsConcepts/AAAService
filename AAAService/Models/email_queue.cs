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
    
    public partial class email_queue
    {
        public System.Guid guid { get; set; }
        public string who_from { get; set; }
        public string who_to { get; set; }
        public string subject { get; set; }
        public string msgbody { get; set; }
        public System.DateTime date_in { get; set; }
        public Nullable<System.Guid> sender_guid { get; set; }
        public bool processed { get; set; }
        public bool is_html { get; set; }
    }
}
