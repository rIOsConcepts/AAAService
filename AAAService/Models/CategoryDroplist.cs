using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AAAService.Models
{
    public class CategoryDroplist
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public SelectList CategoryList { get; set; }

    }
}
