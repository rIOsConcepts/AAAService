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
    
    public partial class ServiceCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ServiceCategory()
        {
            this.service_tickets = new HashSet<service_tickets>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string code { get; set; }
        public string CfdataCode { get; set; }
        public string OldCode { get; set; }
        public Nullable<int> list_num { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<bool> isAlert { get; set; }
        public string AlertMessage { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<service_tickets> service_tickets { get; set; }
    }
}
