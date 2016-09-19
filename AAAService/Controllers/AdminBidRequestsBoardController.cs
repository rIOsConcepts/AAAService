using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AAAService.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using AAAService.Helpers;

namespace AAAService.Controllers
{
    [Authorize(Roles = "SiteAdmin")]
    public class AdminBidRequestsBoardController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        public ActionResult Index()
        {
            var mylist = from c in db.user_to_location
                         select c;

            var x = mylist.Count();
            var mylocationnguid = mylist.Count() > 0 ? mylist.First().location_guid : Guid.NewGuid();

            var mylist2 = from d in db.locationinfoes
                          where d.guid.Equals(mylocationnguid)
                          select d;

            var mycompanyguid = mylist2.Count() > 0 ? mylist2.First().parentguid : Guid.NewGuid();

            if (mylocationnguid.Equals(mycompanyguid))
            {
                x = 2;
            }

            if (mylist == null)
            {
                return View("Details");
            }
            else
            {
                if (x > 1)
                {
                    var multilocs = from s in db.Bid_Requests_View
                                    select s;

                    multilocs = multilocs.Where(s => s.parent_company_guid == mycompanyguid).OrderByDescending(s => s.bid_num);
                    return View(multilocs.ToList<Bid_Requests_View>());
                }
                else
                {
                    var singleloc = from s in db.Bid_Requests_View
                                    select s;

                    singleloc = singleloc.Where(s => s.service_location_guid == (mylocationnguid)).OrderByDescending(s => s.bid_num);
                    var view = singleloc.ToList<Bid_Requests_View>();

                    foreach (var item in view)
                    {
                        item.order_datetime = item.order_datetime.Date;
                    }

                    return View(view);
                }
            }
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            

            var bid_requests = db.bid_requests.Find(id);
            ViewBag.CreatedBy = Helpers.SvcHelper.getCreatedby(bid_requests.created_by_user_guid);
            TempData["BidGuid"] = id;
            ViewBag.BidNumber = bid_requests.bid_num;
            ViewBag.StatusName = bid_requests.StatusName;
            ViewBag.LocationName = bid_requests.LocationName;

            var locationInfo = db.locationinfoes.Where(o => o.active == true && o.guid == bid_requests.service_location_guid);

            if (locationInfo.Count() > 0)
            {
                var locationInfoToList = locationInfo.ToList()[0];
                var companyGUID = locationInfoToList.parentguid;
                bid_requests.LocationName = locationInfoToList.name;
                var companyInfo = db.Companies.Where(o => o.active == true && o.guid == companyGUID);

                if (companyInfo.Count() > 0)
                {
                    var companyName = companyInfo.ToList()[0].name;
                    bid_requests.CompanyName = companyName;
                    //ViewBag.CompanyGUID = companyGUID;
                }
            }

            if (bid_requests == null)
            {
                return HttpNotFound();
            }

            var gparentGuid = bid_requests.parent_company_guid;
            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name", bid_requests.PriorityID);
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", bid_requests.StatusID);
            
            return View(bid_requests);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bid_requests = db.bid_requests.Find(id);
            var lastuserguid = Helpers.UserHelper.getUserGuid();

            var myPriStatus = Request.Form["PriorityName"];
            var myStatus = Request.Form["StatusName"];
            //var myNotes = Request.Form["notes"];

            if (TryUpdateModel(bid_requests, "",
            new string[] { "order_datetime", "complete_datetime", "complete_datetime", "CompanyName", "parent_company_guid", "LocationName", "service_location_guid", "problem_summary", "problem_details", "location_contact_name", "location_contact_phone", "location_contact_phone_night", "notes", "closed_datetime", "active", "PriorityID", "StatusID" }))
            {
                try
                {
                    bid_requests.PriorityStatus = myPriStatus;
                    bid_requests.StatusName = myStatus;
                    //bid_requests.notes = myNotes;
                    bid_requests.last_update_datetime = DateTime.Now;
                    bid_requests.last_updated_by_user_guid = lastuserguid;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            var gparentGuid = bid_requests.parent_company_guid;
            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name", bid_requests.PriorityID);
            ViewBag.StatusID = new SelectList(db.StatusLists, "ID", "Name", bid_requests.StatusID);

            return View(bid_requests);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bid_requests = db.bid_requests.Find(id);

            if (bid_requests == null)
            {
                return HttpNotFound();
            }

            return View(bid_requests);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bid_requests = db.bid_requests.Find(id);

            if (bid_requests == null)
            {
                return HttpNotFound();
            }

            return View(bid_requests);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var bid_requests = db.bid_requests.Find(id);
            db.bid_requests.Remove(bid_requests);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}