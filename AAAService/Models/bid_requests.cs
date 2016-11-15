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
    
    public partial class bid_requests
    {
        public string CompanyName { get; set; }
        public string LocationName { get; set; }
        public int bid_num { get; set; }
        public System.Guid guid { get; set; }
        public System.DateTime order_datetime { get; set; }
        public Nullable<System.DateTime> complete_datetime { get; set; }
        public System.DateTime last_update_datetime { get; set; }
        public System.Guid parent_company_guid { get; set; }
        public System.Guid service_location_guid { get; set; }
        public System.Guid created_by_user_guid { get; set; }
        public System.Guid last_updated_by_user_guid { get; set; }
        public string problem_summary { get; set; }
        public string problem_details { get; set; }
        public string location_contact_name { get; set; }
        public string location_contact_phone { get; set; }
        public string location_contact_phone_night { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> closed_datetime { get; set; }
        public bool active { get; set; }
        public string PriorityStatus { get; set; }
        public Nullable<int> PriorityID { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> StatusID { get; set; }
    
        public virtual locationinfo locationinfo { get; set; }
        public virtual PriorityList PriorityList { get; set; }
        public virtual StatusList StatusList { get; set; }
        public virtual Company Company { get; set; }
    }
}
