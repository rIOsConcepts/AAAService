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
    
    public partial class service_boardNew
    {
        public Nullable<int> age { get; set; }
        public string service_location_name { get; set; }
        public int job_number { get; set; }
        public System.DateTime order_datetime { get; set; }
        public string problem_summary { get; set; }
        public string company_name { get; set; }
        public System.Guid service_location_guid { get; set; }
        public System.Guid parent_company_guid { get; set; }
        public System.Guid guid { get; set; }
        public Nullable<System.DateTime> complete_datetime { get; set; }
        public string region { get; set; }
        public string category { get; set; }
        public string service_provider { get; set; }
        public string city { get; set; }
        public Nullable<System.DateTime> dispatch_datetime { get; set; }
        public Nullable<System.DateTime> accepted_datetime { get; set; }
        public string cust_po_num { get; set; }
        public string cost_code { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
}
