using System;
using System.ComponentModel.DataAnnotations;

namespace AAAService.Models.ReportBuilderModel
{
    public class ReportInformation
    {
        [Required]
        [Display(Name = "Report Name")]
        public string ReportName;

        [Display(Name = "From")]
        public DateTime From;

        [Display(Name = "To")]
        public DateTime To;
    }

    public class Companies
    {
        public Guid GUID;
        public string Name;
    }

    public class Locations
    {
        public Guid GUID;
        public string Name;
    }

    public class ReportBuilderFields
    {
        public int Id;
        public string Name;
    }
}