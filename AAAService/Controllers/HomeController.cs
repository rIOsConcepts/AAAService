using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AAAService.Models;

namespace AAAService.Controllers
{   
    public class HomeController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        public ActionResult Index(string returnUrl, string subdomain)
        {
            ViewBag.Subdomain = subdomain;
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
            var calendars = db.Calendars.Find(4);
            ViewBag.CalendarPath = calendars.Path;
            return View();
        }

        public ActionResult Help()
        {
            ViewBag.Message = "Help";

            return View();
        }
    }
}