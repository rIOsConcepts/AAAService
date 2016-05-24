using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAAService.ViewModels
{
    class CreateTicketVM
    {   
        //hidden or calculated fields
        public int id { get; set; }
        public System.Guid guid { get; set; }
        public int job_number { get; set; }
        public System.DateTime order_datetime { get; set; }
        public System.DateTime last_update_datetime { get; set; }
        public System.Guid parent_company_guid { get; set; }
        public System.Guid service_location_guid { get; set; }
        public System.Guid created_by_user_guid { get; set; }
        public System.Guid last_updated_by_user_guid { get; set; }
        public string ServiceCategory { get; set; }
        public string Region { get; set; }
        public string PriorityStatus { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string Location { get; set; }
        public string City { get; set; }


        //visible fields
        public string location_contact_name { get; set; }
        public string location_contact_phone { get; set; }
        public string location_contact_phone_night { get; set; }
        public string cost_code { get; set; }
        public string cust_po_num { get; set; }
        public Nullable<int> PriorityID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string EQmodel { get; set; }
        public string EQserial { get; set; }
        public string EQProbDesc { get; set; }
        public string problem_summary { get; set; }
        public string problem_details { get; set; }
       
       

    }
}
