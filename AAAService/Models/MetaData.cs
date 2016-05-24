using System;
using System.ComponentModel.DataAnnotations;

namespace AAAService.Models
{
    public class service_ticketsMetadata
    {
        
        [Display(Name = "Job#")]
        public int job_number;

        
        [Display(Name = "Requested")]
        public System.DateTime order_datetime;

        [Display(Name = "Completed")]
        public Nullable<System.DateTime> complete_datetime;

        [Display(Name = "Last Updated")]
        public System.DateTime last_update_datetime ;
        [Required]
        [Display(Name = "Problem Summary")]
        public string problem_summary ;

        [Required]
        
        [Display(Name = "Problem Details")]
        public string problem_details ;

        [Required]
        [Display(Name = "Company")]
        public string CompanyID;

        [Required]
        [Display(Name = "Requester")]
        public string location_contact_name ;
        [Required]
        [Display(Name = "Phone")]
        public string location_contact_phone ;
        [Required]
        [Display(Name = "After Hours Phone")]
        public string location_contact_phone_night ;

        [Display(Name = "Resolution Notes")]
        public string notes ;

        [Display(Name = "Closed")]
        public Nullable<System.DateTime> closed_datetime ;

        [Display(Name = "Active")]
        public bool active ;
        [Required]
        [Display(Name = "Cost Center#")]
        public string cost_code ;

        [Display(Name = "Service Provider")]
        public string service_provider ;

        [Display(Name = "Dispatched")]
        public Nullable<System.DateTime> dispatch_datetime ;

        [Display(Name = "Accepted")]
        public Nullable<System.DateTime> accepted_datetime ;

        [Display(Name = "Internal Notes")]
        public string internal_notes ;

        [Display(Name = "Customer PO#")]
        public string cust_po_num ;

        [Display(Name = "Total Billing")]
        public Nullable<long> total_billing ;

        [Display(Name = "Service Category")]
        public string ServiceCategory ;

        [Required]
        public int CategoryID;

        [Display(Name = "Priority Level")]
        public string PriorityStatus ;

        [Required]
        [Display(Name = "Priority Level")]
        public string PriorityID;

        [Display(Name = "Status")]
        public string StatusName ;

        [Display(Name = "Status")]
        public string StatusID;



    }

    public class bid_requestsMetadata
    {
        [Display(Name = "Bid Request#")]
        public int bid_num ;
        public System.Guid guid ;

        [Display(Name = "Request Date")]
        public System.DateTime order_datetime ;

        [Display(Name = "Completed")]
        public Nullable<System.DateTime> complete_datetime ;

        [Display(Name = "Last Updated")]
        public System.DateTime last_update_datetime ;

        [Display(Name = "Company ID")]
        public System.Guid parent_company_guid ;

        [Display(Name = "Service Location ID")]
        public System.Guid service_location_guid ;

        [Display(Name = "Created By ID")]
        public System.Guid created_by_user_guid ;

        [Display(Name = "Updated By ID")]
        public System.Guid last_updated_by_user_guid ;

        [Required]
        [Display(Name = "Request Summary")]
        public string problem_summary ;

        [Required]
        [Display(Name = "Request Details")]
        public string problem_details ;

        [Required]
        [Display(Name = "Who should we contact?")]
        public string location_contact_name ;

        [Required]
        [Display(Name = "Phone")]
        public string location_contact_phone ;

        [Display(Name = "After Hours Phone")]
        public string location_contact_phone_night ;

        [Display(Name = "Notes")]
        public string notes ;

        [Display(Name = "Closed")]
        public Nullable<System.DateTime> closed_datetime ;

        [Display(Name = "Active")]
        public bool active ;

        
        [Display(Name = "Priority Level")]
        public string PriorityStatus ;
        [Required]
        public Nullable<int> PriorityID ;
       
        [Display(Name = "Status")]
        public string StatusName ;
        public Nullable<int> StatusID ;


    }

    public class CompanyMetadata

    {
        [Required]
        [Display(Name = "Name")]
        public string name ;

        [Required]
        [Display(Name = "Address1")]
        public string addressline1 ;
        [Display(Name = "Address2")]
        public string addressline2 ;

        [Required]
        [Display(Name = "City")]
        public string city ;

        [Required]
        [Display(Name = "State")]
        public string state ;

        [Required]
        [Display(Name = "ZipCode")]
        public int zip ;
        [Display(Name = "CF Company#")]
        public string cf_company_num ;
        [Display(Name = "CF Location#")]
        public string cf_location_num ;
        [Display(Name = "Region")]
        public string region ;
        [Display(Name = "Email All")]
        public Nullable<bool> email_all_members ;
        [Display(Name = "Active")]
        public bool active ;
    }

    public class errors_andcommentsMetadata
    {
        [Display(Name = "Comments")]

        public string comments ;

        [Display(Name = "Comment Date")]
        public DateTime comment_datetime;

        [Display(Name = "File Upload")]
        public string fileupload ;

        [Display(Name = "Active")]
        public bool active ;
        public Nullable<System.DateTime> closed ;

        [Display(Name = "Notes")]
        public string notes ;

        [Display(Name = "Please Tell Us How We're Doing")]
        public Nullable<int> ratings ;
        
    }
    public class ServiceCategoryMetadata
    {
        [Display(Name = "Code")]
        public string code;

        [Display(Name = "Old List #")]
        public int list_num;

        [Display(Name = "Active")]
        public bool active;
        [Display(Name = "IsAlert")]
        public bool isAlert; 
    }

    public class locationinfoMetadata
    {
        [Display(Name = "Name")]
        public string name ;
        [Display(Name = "Address1")]
        public string addressline1 ;
        [Display(Name = "Address2")]
        public string addressline2 ;
        [Display(Name = "City")]
        public string city ;
        [Display(Name = "State")]
        public string state ;
        [Display(Name = "Zip")]
        public int zip ;
        [Display(Name = "CompanyID")]
        public Guid parentguid ;
        [Display(Name = "CF Location#")]
        public string cf_location_num ;
        [Display(Name = "CF Company#")]
        public string cf_company_num ;
        [Display(Name = "Region")]
        public string region ;

        [Display(Name = "Email All")]
        public Nullable<bool> email_all_members ;

        [Display(Name = "Active")]
        public bool active ;
    }
    public class PriorityListMetadata
    {
        [Display(Name = "Code")]
        public string code;

        [Display(Name = "Old List#")]
        public int list_num;

        [Display(Name = "Active")]
        public bool active;



    }

   /* public partial class StatusListMetadata
    {
        [Display(Name = "Code")]
        public string code ;
        [Display(Name = "Old List#")]
        public Nullable<int> list_num ;
        [Display(Name = "Active")]
        public Nullable<bool> active ;

    }*/

    }