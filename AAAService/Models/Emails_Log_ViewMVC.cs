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
    
    public partial class Emails_Log_ViewMVC
    {
        public int ID { get; set; }
        public string who_from { get; set; }
        public string subject { get; set; }
        public string msgbody { get; set; }
        public System.DateTime date_in { get; set; }
        public string date_in_no_time { get; set; }
        public string location_name { get; set; }
        public string Email { get; set; }
        public string sent_to_full_name { get; set; }
        public System.Guid guid { get; set; }
        public string who_to { get; set; }
    }
}
