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
            var multilocs = from s in db.Bid_Requests_View
                            select s;

            multilocs = multilocs.OrderByDescending(s => s.bid_num);
            var view = multilocs.ToList<Bid_Requests_View>();

            foreach (var item in view)
            {
                item.order_datetime = item.order_datetime.Date;
            }

            return View(view);
        }

        // GET: BidRequests/Create
        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(db.Companies.Where(o => o.active == true).OrderBy(o => o.name), "guid", "name");
            ViewBag.LocationDD = new SelectList(db.locationinfoes.Where(o => o.active == true).OrderBy(o => o.name), "guid", "name");
            var userGUID = Helpers.UserHelper.getUserGuid();
            var view = new bid_requests();            
            var primaryContactASPNetUsers = db.AspNetUsers.Where(o => o.guid == userGUID);
            var primaryContactPhoneNum = db.phone_num.Where(o => o.user_guid == userGUID);

            if (primaryContactASPNetUsers.Count() > 0)
            {
                view.location_contact_name = primaryContactASPNetUsers.ToList()[0].fname + " " + primaryContactASPNetUsers.ToList()[0].lname;
            }

            if (primaryContactPhoneNum.Count() > 0)
            {
                view.location_contact_phone = primaryContactPhoneNum.ToList()[0].phone_day;
                view.location_contact_phone_night = primaryContactPhoneNum.ToList()[0].phone_night;
            }

            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true), "ID", "Name");
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "problem_summary,problem_details,CompanyName,LocationName,location_contact_name,location_contact_phone,location_contact_phone_night,PriorityID,")] bid_requests bid_requests)
        {
            var strLocationGuid = Request.Form["LocationDD"];
            var locationGUID = Guid.Parse(strLocationGuid);
            var myPriStatus = Request.Form["PriStatus"];
            int maxNumBQ = Helpers.SvcHelper.getBidMax() + 1;
            var found = db.locationinfoes.FirstOrDefault(u => u.guid.Equals(locationGUID));
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
                bid_requests.service_location_guid = locationGUID;
                bid_requests.StatusID = 9;
                bid_requests.StatusName = "Open";
                bid_requests.created_by_user_guid = LastUserGUID;
                bid_requests.guid = Guid.NewGuid();
                bid_requests.PriorityStatus = db.PriorityLists.Where(o => o.ID == bid_requests.PriorityID).Select(o => o.Name).ToList()[0];
                bid_requests.LocationName = db.locationinfoes.Where(o => o.guid == bid_requests.service_location_guid).Select(o => o.name).ToList()[0];
                bid_requests.CompanyName = db.Companies.Where(o => o.guid == bid_requests.parent_company_guid).Select(o => o.name).ToList()[0];
                db.bid_requests.Add(bid_requests);
                db.SaveChanges();

                try
                {
                    var email = new Correspondence.Mail();
                    var user = db.AspNetUsers.Where(o => o.guid == bid_requests.last_updated_by_user_guid).ToList()[0];
                    var parentCompanyName = db.Companies.Where(o => o.active == true && o.guid == found.parentguid);

                    var body = "Bid Number #: " + bid_requests.bid_num + "\r\n" +
                               "Request Date: " + bid_requests.order_datetime.ToShortDateString() + "\r\n" +
                               "Request Time: " + bid_requests.order_datetime.ToShortTimeString() + "\r\n" +
                               "CF Data Company #: " + (parentCompanyName.Count() > 0 ? parentCompanyName.ToList()[0].cf_company_num : "") + "\r\n" +
                               "Parent Company Name: " + (parentCompanyName.Count() > 0 ? parentCompanyName.ToList()[0].name : "") + "\r\n" +
                               "CF Data Service Location #: " + found.cf_location_num + "\r\n" +
                               "Service Location Name: " + found.cf_location_num + "\r\n" +
                               "Service Location Address Line 1: " + (found.addressline1 != null ? found.addressline1.ToUpper() : "") + "\r\n" +
                               "Service Location Address Line 2: " + (found.addressline2 != null ? found.addressline2.ToUpper() : "") + "\r\n" +
                               "Service Location City: " + (found.city != null ? found.city.ToUpper() : "") + "\r\n" +
                               "Service Location State: " + (found.state != null ? found.state.ToUpper() : "") + "\r\n" +
                               "Service Location Zip: " + (found.zip > 0 ? found.zip.ToString() : "") + "\r\n" +
                               "Request Priority Code: " + bid_requests.PriorityID + "\r\n" +
                               "Request Summary: " + bid_requests.problem_summary.ToUpper() + "\r\n" +
                               "Request Details: " + bid_requests.problem_details.ToUpper() + "\r\n" +
                               "Request Created By: " + user.fname.ToUpper() + " " + user.lname.ToUpper() + "\r\n" +
                               "Person To Contact: " + found.name.ToUpper() + "\r\n" +
                               "Contact Phone: " + bid_requests.location_contact_phone + "\r\n" +
                               "Contact After Hours Phone: " + bid_requests.location_contact_phone_night + "\r\n";

                    email.Send(subject: "Web Portal Bid Request Submitted", body: body, email: user.Email);
                }
                catch (Exception e)
                {
                    System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                }

                return RedirectToAction("Index", "AdminBidRequestsBoard");
            }


            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name", bid_requests.PriorityID);
            var companyGUID = found.parentguid;
            ViewBag.CompanyGUID = companyGUID;
            return View(bid_requests);
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
                    bid_requests.PriorityStatus = db.PriorityLists.Where(o => o.ID == bid_requests.PriorityID).Select(o => o.Name).ToList()[0];
                    bid_requests.StatusName = myStatus;
                    //bid_requests.notes = myNotes;
                    bid_requests.last_update_datetime = DateTime.Now;
                    bid_requests.last_updated_by_user_guid = lastuserguid;
                    db.SaveChanges();

                    try
                    {
                        var email = new Correspondence.Mail();
                        var user = db.AspNetUsers.Where(o => o.guid == bid_requests.last_updated_by_user_guid).ToList()[0];
                        var found = db.locationinfoes.FirstOrDefault(u => u.guid.Equals(bid_requests.service_location_guid));
                        var parentCompanyName = db.Companies.Where(o => o.active == true && o.guid == found.parentguid);

                        var body = "Bid Number #: " + bid_requests.bid_num + "\r\n" +
                                   "Request Date: " + bid_requests.order_datetime.ToShortDateString() + "\r\n" +
                                   "Request Time: " + bid_requests.order_datetime.ToShortTimeString() + "\r\n" +
                                   "CF Data Company #: " + (parentCompanyName.Count() > 0 ? parentCompanyName.ToList()[0].cf_company_num : "") + "\r\n" +
                                   "Parent Company Name: " + (parentCompanyName.Count() > 0 ? parentCompanyName.ToList()[0].name : "") + "\r\n" +
                                   "CF Data Service Location #: " + found.cf_location_num + "\r\n" +
                                   "Service Location Name: " + found.cf_location_num + "\r\n" +
                                   "Service Location Address Line 1: " + (found.addressline1 != null ? found.addressline1.ToUpper() : "") + "\r\n" +
                                   "Service Location Address Line 2: " + (found.addressline2 != null ? found.addressline2.ToUpper() : "") + "\r\n" +
                                   "Service Location City: " + (found.city != null ? found.city.ToUpper() : "") + "\r\n" +
                                   "Service Location State: " + (found.state != null ? found.state.ToUpper() : "") + "\r\n" +
                                   "Service Location Zip: " + (found.zip > 0 ? found.zip.ToString() : "") + "\r\n" +
                                   "Request Summary: " + bid_requests.problem_summary.ToUpper() + "\r\n" +
                                   "Request Details: " + bid_requests.problem_details.ToUpper() + "\r\n" +
                                   "Request Created By: " + user.fname.ToUpper() + " " + user.lname.ToUpper() + "\r\n" +
                                   "Person To Contact: " + found.name.ToUpper() + "\r\n" +
                                   "Contact Phone: " + bid_requests.location_contact_phone + "\r\n" +
                                   "Contact After Hours Phone: " + bid_requests.location_contact_phone_night + "\r\n";

                        email.Send(subject: "Web Portal Bid Request Updated", body: body, email: user.Email);
                    }
                    catch (Exception e)
                    {
                        System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                    }

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