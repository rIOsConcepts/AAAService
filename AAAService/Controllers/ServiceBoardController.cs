using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AAAService.Models;
using AAAService.Helpers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace AAAService.Controllers
{
    [Authorize]
    public class ServiceBoardController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();
       

        // GET: ServiceBoard
        public ActionResult Index()
        {   var myuserguid = Helpers.UserHelper.getUserGuid();
            var mylocationguid = Helpers.SvcHelper.getLocation();

            if (mylocationguid == Guid.Parse("6FFB64D7-4D69-4F1C-BC55-5376588A39F4"))
            {
                return RedirectToAction("Edit");
            }

            else
            { 
                var mylist2 = from d in db.locationinfoes
                      where d.guid.Equals(mylocationguid)
                      select d;

                var mycompanyguid = mylist2.First().parentguid;

                var x = Helpers.SvcHelper.getnumLocations();

            if (mylocationguid.Equals(mycompanyguid))
            {
                    var multilocs = from s in db.service_boardNew
                                    select s;
                    multilocs = multilocs.Where(s => s.parent_company_guid == mycompanyguid).OrderByDescending(s => s.job_number);

                    return View(multilocs.ToList<service_boardNew>());
                }

            if (x>0)
                {
                    var multilocs = from s in db.service_boardNew
                                    select s;
                    multilocs = multilocs.Where(s => s.parent_company_guid == mycompanyguid).OrderByDescending(s => s.job_number);

                    return View(multilocs.ToList<service_boardNew>());

                }
                else
                {
                    var singleloc = from s in db.service_boardNew
                                    select s;

                    singleloc = singleloc.Where(s => s.service_location_guid.Equals(mylocationguid)).OrderByDescending(s=>s.job_number);
                    var view = singleloc.ToList<service_boardNew>();
                    return View(view);
                }



            }
           
        }

        // GET: ServiceBoard/Details/5
        public ActionResult Details()
        {
            
            return View();
        }

         
        
       

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
