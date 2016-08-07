using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AAAService.Models.ReportBuilderModel
{
    public class ReportInformation
    {
        [Required]
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }

        [Display(Name = "From")]
        public DateTime From { get; set; }

        [Display(Name = "To")]
        public DateTime To { get; set; }

        public List<Company> Companies { get; set; }
        public List<Location> Locations { get; set; }
        public List<ReportBuilderField> ReportBuilderFields { get; set; }
    }

    public class Company
    {
        public Guid GUID { get; set; }
        public string Name { get; set; }
    }

    public class Location
    {
        public Guid GUID { get; set; }
        public string Name { get; set; }
    }

    public class ReportBuilderField
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}