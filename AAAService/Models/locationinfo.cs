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
    
    public partial class locationinfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public locationinfo()
        {
            this.user_to_location = new HashSet<user_to_location>();
            this.bid_requests = new HashSet<bid_requests>();
            this.user_viewable_locations = new HashSet<user_viewable_locations>();
        }
    
        public System.Guid guid { get; set; }
        public string name { get; set; }
        public string addressline1 { get; set; }
        public string addressline2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public int zip { get; set; }
        public Nullable<System.Guid> parentguid { get; set; }
        public string cf_location_num { get; set; }
        public string cf_company_num { get; set; }
        public string region { get; set; }
        public Nullable<bool> email_all_members { get; set; }
        public bool active { get; set; }
        public Nullable<int> RegionID { get; set; }
    
        public virtual Location_Service_Provider Location_Service_Provider { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<user_to_location> user_to_location { get; set; }
        public virtual Region Region1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<bid_requests> bid_requests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<user_viewable_locations> user_viewable_locations { get; set; }
        public virtual Company Company { get; set; }
    }
}
