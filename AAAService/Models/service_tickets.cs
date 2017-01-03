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
    
    public partial class service_tickets
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public service_tickets()
        {
            this.service_ticket_files = new HashSet<service_ticket_files>();
            this.ST_SL_Edits = new HashSet<ST_SL_Edits>();
        }
    
        public int id { get; set; }
        public System.Guid guid { get; set; }
        public int job_number { get; set; }
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
        public string cost_code { get; set; }
        public string EQmodel { get; set; }
        public string EQserial { get; set; }
        public string EQProbDesc { get; set; }
        public string service_provider { get; set; }
        public Nullable<System.DateTime> dispatch_datetime { get; set; }
        public Nullable<System.DateTime> accepted_datetime { get; set; }
        public string internal_notes { get; set; }
        public string cust_po_num { get; set; }
        public Nullable<long> total_billing { get; set; }
        public string ServiceCategory { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Region { get; set; }
        public string PriorityStatus { get; set; }
        public Nullable<int> PriorityID { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
    
        public virtual PriorityList PriorityList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<service_ticket_files> service_ticket_files { get; set; }
        public virtual StatusList StatusList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ST_SL_Edits> ST_SL_Edits { get; set; }
        public virtual ServiceCategory ServiceCategory1 { get; set; }
    }
}
