using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AAAService.Models;

namespace AAAService.Controllers
{   [Authorize]
    public class BidRequestsController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: BidRequests
        public ActionResult Index()
        {
            var bid_requests = db.bid_requests.Include(b => b.Company).Include(b => b.locationinfo).Include(b => b.PriorityList).Include(b => b.StatusList);
            return View();
        }

        // GET: BidRequests/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bid_requests bid_requests = db.bid_requests.Find(id);
            if (bid_requests == null)
            {
                return HttpNotFound();
            }
            return View(bid_requests);
        }

        // GET: BidRequests/Create
        public ActionResult Create(Guid? id)
        { 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userGUID = Helpers.UserHelper.getUserGuid();
            var locationsOfUser = db.user_to_location.Where(o => o.user_guid == userGUID).Select(o => o.location_guid.ToString());

            if (locationsOfUser.Count() > 1)
            {
                TempData["SeveralLocations"] = true;
            }
            else
            {
                TempData["SeveralLocations"] = false;
            }

            var view = new bid_requests();
            var locationInfo = db.locationinfoes.Where(o => o.active == true && o.guid == id);

            if (locationInfo.Count() > 0)
            {
                var locationInfoToList = locationInfo.ToList()[0];
                var companyGUID = locationInfoToList.parentguid;
                view.LocationName = locationInfoToList.name;
                var companyInfo = db.Companies.Where(o => o.active == true && o.guid == companyGUID);

                if (companyInfo.Count() > 0)
                {
                    var companyName = companyInfo.ToList()[0].name;
                    view.CompanyName = companyName;
                    ViewBag.CompanyGUID = companyGUID;
                }

                if (locationInfo.Count() == 1)
                {
                    var primaryContact = db.user_to_location.Where(o => o.location_guid == id && o.primary_contact_bit == true);

                    if (primaryContact.Count() > 0)
                    {
                        var primaryContactUserGUID = primaryContact.ToList()[0].user_guid;
                        var primaryContactASPNetUsers = db.AspNetUsers.Where(o => o.guid == primaryContactUserGUID);
                        var primaryContactPhoneNum = db.phone_num.Where(o => o.user_guid == primaryContactUserGUID);

                        if (primaryContactASPNetUsers.Count() > 0)
                        {
                            view.location_contact_name = primaryContactASPNetUsers.ToList()[0].fname + " " + primaryContactASPNetUsers.ToList()[0].lname;
                        }

                        if (primaryContactPhoneNum.Count() > 0)
                        {
                            view.location_contact_phone = primaryContactPhoneNum.ToList()[0].phone_day;
                            view.location_contact_phone_night = primaryContactPhoneNum.ToList()[0].phone_night;
                        }
                    }
                }
            }

            TempData["LocationGuid"] = id;
            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true), "ID", "Name");
            return View(view);
        }

        // POST: BidRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "problem_summary,problem_details,CompanyName,LocationName,location_contact_name,location_contact_phone,location_contact_phone_night,PriorityID,")] bid_requests bid_requests)
        {
            var mynewguid = TempData.Peek("LocationGuid").ToString();
            Guid mytest = Guid.Parse(mynewguid);

            var myPriStatus = Request.Form["PriStatus"];
            int maxNumBQ = Helpers.SvcHelper.getBidMax() + 1;
            var found = db.locationinfoes.FirstOrDefault(u => u.guid.Equals(mytest));
            var LocationName = found.name;
            var ParentLoactionGuid = found.parentguid.ToString();
            Guid mypappy = Guid.Parse(ParentLoactionGuid);
            var LocationRegion = found.region;
            var LocationCity = found.city;
            var LastUserGUID = Helpers.UserHelper.getUserGuid();
            if (ModelState.IsValid)
            {
                bid_requests.bid_num = maxNumBQ;
                bid_requests.PriorityStatus = myPriStatus;
                bid_requests.active = true;
                bid_requests.parent_company_guid = mypappy;
                bid_requests.order_datetime = DateTime.Now;
                bid_requests.last_updated_by_user_guid = LastUserGUID;
                bid_requests.last_update_datetime = DateTime.Now;
                bid_requests.service_location_guid = mytest;
                bid_requests.StatusID = 9;
                bid_requests.StatusName = "Open";
                bid_requests.created_by_user_guid = LastUserGUID;
                bid_requests.guid = Guid.NewGuid();
                db.bid_requests.Add(bid_requests);
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Index", "BidRequestsBoard");
            }

           
            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name", bid_requests.PriorityID);
           
            return View(bid_requests);
        }

        // GET: BidRequests/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bid_requests bid_requests = db.bid_requests.Find(id);
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
                }
            }

            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true), "ID", "Name", bid_requests.PriorityID);
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", bid_requests.StatusID);

            return View(bid_requests);
        }

        // POST: BidRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bidToUpdate = db.bid_requests.Find(id);
            var mylastuserguid = Helpers.UserHelper.getUserGuid();

            if (TryUpdateModel(bidToUpdate, "",
                new string[] { "problem_details", "problem_summary", "location_contact_name", "location_contact_phone", "location_contact_phone_night", }))
            {
                try
                {
                    bidToUpdate.last_updated_by_user_guid = mylastuserguid;
                    bidToUpdate.last_update_datetime = DateTime.Now;
                    //var priorityID = int.Parse(Request.Form["PriorityID"]);
                    //bidToUpdate.PriorityID = priorityID;
                    //bidToUpdate.PriorityStatus = db.PriorityLists.Where(o => o.active == true && o.ID == priorityID).ToList()[0].Name;
                    //Users not allowed to change Status, therefore commenting out code below
                    //var statusID = int.Parse(Request.Form["StatusID"]);
                    //bidToUpdate.StatusID = statusID;
                    //bidToUpdate.StatusName = db.StatusLists.Where(o => o.active == true && o.ID == statusID).ToList()[0].Name;

                    var mytime = DateTime.Now.ToString();
                    var newNotes = Request.Form["ProbSum"];

                    if (newNotes != "")
                    {
                        var problem_details = Request.Form["problem_details"];
                        var internalNotes = Request.Form["internal_notes"];

                        if (problem_details != "")
                        {
                            problem_details = "Updated " + mytime + System.Environment.NewLine + newNotes + System.Environment.NewLine + System.Environment.NewLine + problem_details;
                        }
                        else
                        {
                            problem_details = Request.Form["ProbSum"];
                        }

                        bidToUpdate.problem_details = problem_details;
                    }

                    db.SaveChanges();

                    //return RedirectToAction("Index");
                    return RedirectToAction("Index", "BidRequestsBoard");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

            }

            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name", bidToUpdate.PriorityID);
           
            return View(bidToUpdate);
        }

        // GET: BidRequests/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bid_requests bid_requests = db.bid_requests.Find(id);
            if (bid_requests == null)
            {
                return HttpNotFound();
            }
            return View(bid_requests);
        }

        // POST: BidRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            bid_requests bid_requests = db.bid_requests.Find(id);
            db.bid_requests.Remove(bid_requests);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Action result for ajax call
        [HttpPost]
        public ActionResult GetServiceLocation(string companyGUID)
        {
            SelectList obg;

            if (User.IsInRole("CorpAdmin"))
            {
                obg = new SelectList(db.locationinfoes.Where(o => o.parentguid.ToString() == companyGUID && o.active == true).OrderBy(o => o.name), "guid", "Name");
            }
            else
            {
                var userGUID = Helpers.UserHelper.getUserGuid();
                var locationsOfUser = db.user_to_location.Where(o => o.user_guid == userGUID).Select(o => o.location_guid.ToString()).ToList();
                obg = new SelectList(db.locationinfoes.Where(o => o.parentguid.ToString() == companyGUID && o.active == true && locationsOfUser.Contains(o.guid.ToString())).OrderBy(o => o.name), "guid", "Name");
            }

            return Json(obg);
        }

        public ActionResult SetServiceLocation(string locationGUID)
        {
            TempData["LocationGuid"] = locationGUID;
            return null;
        }
    }
}