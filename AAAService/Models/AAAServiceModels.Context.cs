﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class aaahelpEntities : DbContext
    {
        public aaahelpEntities()
            : base("name=aaahelpEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<email_queue> email_queue { get; set; }
        public virtual DbSet<locationinfo> locationinfoes { get; set; }
        public virtual DbSet<PriorityList> PriorityLists { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<ST_SL_Edits> ST_SL_Edits { get; set; }
        public virtual DbSet<StatusList> StatusLists { get; set; }
        public virtual DbSet<service_boardNew> service_boardNew { get; set; }
        public virtual DbSet<user_viewable_locations_view> user_viewable_locations_view { get; set; }
        public virtual DbSet<Bid_Requests_View> Bid_Requests_View { get; set; }
        public virtual DbSet<Comments_view> Comments_view { get; set; }
        public virtual DbSet<Emails_Log_ViewMVC> Emails_Log_ViewMVC { get; set; }
        public virtual DbSet<Location_Mgt_ViewMVC> Location_Mgt_ViewMVC { get; set; }
        public virtual DbSet<LocationEmails_ViewMVC> LocationEmails_ViewMVC { get; set; }
        public virtual DbSet<User_Assigned_Locations_View> User_Assigned_Locations_View { get; set; }
        public virtual DbSet<User_Info_ViewMVC> User_Info_ViewMVC { get; set; }
        public virtual DbSet<Location_Service_Provider> Location_Service_Provider { get; set; }
        public virtual DbSet<user_to_location> user_to_location { get; set; }
        public virtual DbSet<service_ticket_files> service_ticket_files { get; set; }
        public virtual DbSet<email_log> email_log { get; set; }
        public virtual DbSet<bid_requests> bid_requests { get; set; }
        public virtual DbSet<service_tickets> service_tickets { get; set; }
        public virtual DbSet<user_viewable_locations> user_viewable_locations { get; set; }
        public virtual DbSet<phone_num> phone_num { get; set; }
        public virtual DbSet<Phone_Numbers> Phone_Numbers { get; set; }
        public virtual DbSet<errors_and_comments> errors_and_comments { get; set; }
        public virtual DbSet<CommentRating> CommentRatings { get; set; }
        public virtual DbSet<Site_Lists_Hide_Item> Site_Lists_Hide_Item { get; set; }
        public virtual DbSet<EmailSetup> EmailSetups { get; set; }
        public virtual DbSet<LocationEmails_viewMVC1> LocationEmails_viewMVC1 { get; set; }
        public virtual DbSet<service_tickets_viewMVC> service_tickets_viewMVC { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<OtherStatusList> OtherStatusLists { get; set; }
        public virtual DbSet<ExportToCF> ExportToCFs { get; set; }
    
        public virtual ObjectResult<ParentLocationInfoFromUsrGUID_Result> ParentLocationInfoFromUsrGUID(Nullable<System.Guid> usrGUID)
        {
            var usrGUIDParameter = usrGUID.HasValue ?
                new ObjectParameter("usrGUID", usrGUID) :
                new ObjectParameter("usrGUID", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ParentLocationInfoFromUsrGUID_Result>("ParentLocationInfoFromUsrGUID", usrGUIDParameter);
        }
    
        public virtual ObjectResult<ReturnParentLocations_Result> ReturnParentLocations(string sessionid)
        {
            var sessionidParameter = sessionid != null ?
                new ObjectParameter("sessionid", sessionid) :
                new ObjectParameter("sessionid", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReturnParentLocations_Result>("ReturnParentLocations", sessionidParameter);
        }
    }
}
