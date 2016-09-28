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
        {
            var myuserguid = Helpers.UserHelper.getUserGuid();
            var mylocationguid = Helpers.SvcHelper.getLocation();

            if (mylocationguid == Guid.Parse("6FFB64D7-4D69-4F1C-BC55-5376588A39F4"))
            {
                //return RedirectToAction("Edit");
                return View(new List<service_boardNew>());
            }
            else
            { 
                var mylist2 = from d in db.locationinfoes
                              where d.guid.Equals(mylocationguid)
                              select d;

                var mycompanyguid = mylist2.First().parentguid;
                //var x = Helpers.SvcHelper.getnumLocations();
                var mylist = from c in db.user_to_location
                             where c.user_guid.Equals(myuserguid)
                             select c;

                var x = mylist.Count();

                if (mylocationguid.Equals(mycompanyguid))
                {
                    var multilocs = from s in db.service_boardNew
                                    select s;

                    if (User.IsInRole("CorpAdmin"))
                    {
                        multilocs = multilocs.Where(s => s.parent_company_guid == mycompanyguid).OrderByDescending(s => s.job_number);
                    }
                    else
                    {
                        var locationsOfUser = mylist.Select(o => o.location_guid.ToString()).ToList();
                        multilocs = multilocs.Where(s => locationsOfUser.Contains(s.service_location_guid.ToString())).OrderByDescending(s => s.job_number);
                    }

                    return View(multilocs.ToList<service_boardNew>());
                }

                if (x > 0)
                {
                    var multilocs = from s in db.service_boardNew
                                    select s;

                    if (User.IsInRole("CorpAdmin"))
                    {
                        multilocs = multilocs.Where(s => s.parent_company_guid == mycompanyguid).OrderByDescending(s => s.job_number);
                    }
                    else
                    {
                        var locationsOfUser = mylist.Select(o => o.location_guid.ToString()).ToList();
                        multilocs = multilocs.Where(s => locationsOfUser.Contains(s.service_location_guid.ToString())).OrderByDescending(s => s.job_number);
                    }

                    return View(multilocs.ToList<service_boardNew>());
                }
                else
                {
                    var singleloc = from s in db.service_boardNew
                                    select s;

                    singleloc = singleloc.Where(s => s.service_location_guid.Equals(mylocationguid)).OrderByDescending(s=>s.job_number);
                    var view = singleloc.ToList<service_boardNew>();

                    foreach (var item in view)
                    {
                        item.order_datetime = item.order_datetime.Date;
                        item.dispatch_datetime = item.dispatch_datetime?.Date;
                        item.complete_datetime = item.complete_datetime?.Date;
                    }

                    return View(view);
                }
            }           
        }

        //Action result for ajax call 
        [HttpPost]
        public ActionResult GetStatuses()
        {
            SelectList obj = new SelectList(db.StatusLists.Where(o => o.active == true).OrderBy(o => o.Name), "ID", "Name");
            return Json(obj);
        }

        //Action result for ajax call 
        [HttpPost]
        public ActionResult GetPriorities()
        {
            SelectList obj = new SelectList(db.PriorityLists.Where(o => o.active == true).OrderBy(o => o.Name), "ID", "Name");
            return Json(obj);
        }

        //Action result for ajax call 
        [HttpPost]
        public ActionResult GetServiceCategories()
        {
            SelectList obj = new SelectList(db.ServiceCategories.Where(o => o.active == true).OrderBy(o => o.Name), "ID", "Name");
            return Json(obj);
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