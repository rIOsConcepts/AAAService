using System;
using System.ComponentModel.DataAnnotations;


namespace AAAService.Models
{
    [MetadataType(typeof(service_ticketsMetadata))]
    public partial class service_tickets
    {
    }

    [MetadataType(typeof(bid_requestsMetadata))]
    public partial class bid_requests
    {
    }


    [MetadataType(typeof(CompanyMetadata))]

    public partial class Company
    {
    }

    [MetadataType(typeof(errors_andcommentsMetadata))]
    public partial class errors_and_comments
    {
    }

    [MetadataType(typeof(ServiceCategoryMetadata))]
    public partial  class ServiceCategory
    {
    }

    [MetadataType(typeof(locationinfoMetadata))]
    public partial class locationinfo
    {

    }

    [MetadataType(typeof(PriorityListMetadata))]
    public partial class PriorityList

    {

    }

   // [MetadataType(typeof(StatusListMetadata))]
   // public partial class StatusList
   // {
   //
  //  }

}
