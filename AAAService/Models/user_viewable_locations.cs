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
    
    public partial class user_viewable_locations
    {
        public System.Guid guid { get; set; }
        public System.Guid user_guid { get; set; }
        public System.Guid location_guid { get; set; }
    
        public virtual locationinfo locationinfo { get; set; }
    }
}
