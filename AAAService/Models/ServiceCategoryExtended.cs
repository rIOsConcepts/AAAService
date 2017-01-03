using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AAAService.Models
{
    public class ServiceCategoryExtended : ServiceCategory
    {
        public ServiceCategoryExtended(ServiceCategory serviceCategory)
        {
            this.ID = serviceCategory.ID;
            this.Company = serviceCategory.Company;
            this.Name = serviceCategory.Name;
            this.code = serviceCategory.code;
            this.CfdataCode = serviceCategory.CfdataCode;
            this.OldCode = serviceCategory.OldCode;
            this.list_num = serviceCategory.list_num;
            this.active = serviceCategory.active;
            this.isAlert = serviceCategory.isAlert;
            this.AlertMessage = serviceCategory.AlertMessage;
        }

        public string CompanyName { get; set; }
    }
}