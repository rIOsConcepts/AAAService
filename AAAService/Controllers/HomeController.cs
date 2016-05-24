using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AAAService.Controllers
{
   
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult Maintenance()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult HVAC()
        {
            ViewBag.Message = "HVAC Calendar";

            return View();
        }


        [Authorize]
        public ActionResult FireExt()
        {
            ViewBag.Message = "Annual Fire Extinguisher Service Calendar";

            return View();
        }

        [Authorize]
        public ActionResult Landscape()
        {
            ViewBag.Message = "Landscape Calendar";

            return View();
        }

        [Authorize]
        public ActionResult BankMaintenance()
        {
            ViewBag.Message = "Bank Maintenance Calendar";

            return View();
        }

        public ActionResult Help()
        {
            ViewBag.Message = "Help";

            return View();
        }
    }
}