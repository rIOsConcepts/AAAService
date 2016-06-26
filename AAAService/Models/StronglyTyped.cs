using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AAAService.Models
{
    public class StronglyTyped
    {
        public Guid GUID { get; set; }
        public string Name { get; set; }
        public Guid? ParentGUID { get; set; }
    }
}