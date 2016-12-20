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
using System.IO;

namespace AAAService.Controllers
{
    [Authorize(Roles = "SiteAdmin")]
    public class AdminTicketsController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: AdminTickets
        public ActionResult Index()
        {
            var multilocs = from s in db.service_boardNew
                            select s;

            multilocs = multilocs.OrderByDescending(s => s.job_number);
            var view = multilocs.ToList<service_boardNew>();

            foreach (var item in view)
            {
                item.order_datetime = item.order_datetime.Date;
                item.dispatch_datetime = item.dispatch_datetime?.Date;
                item.complete_datetime = item.complete_datetime?.Date;
            }

            return View(view);
        }
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            var myuserguid = Helpers.UserHelper.getUserGuid();
            var service_ticket_files = db.service_ticket_files;
            Guid myNewGuid = Guid.NewGuid();
            var myticketguid = TempData["TicketGuid"].ToString();
            Guid newTicketGuid = Guid.Parse(myticketguid);
            string filenameguid = myNewGuid.ToString("B");

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    string fileExtension = Path.GetExtension(fileName);
                    //long fileSize2 = new FileInfo(Path.GetFileName(file.FileName)).Length;
                    var fileSize = file.ContentLength;

                    var path = Path.Combine(Server.MapPath("~/Content/service_ticket_uploads"), filenameguid + fileExtension);
                    file.SaveAs(path);

                    using (var db = new aaahelpEntities())
                    {
                        var tickets = db.Set<service_ticket_files>();
                        tickets.Add(new service_ticket_files { guid = myNewGuid, ticket_guid = newTicketGuid, date_in = DateTime.Now, active = true, file_name = fileName, file_ext = fileExtension, file_size = fileSize, user_guid = myuserguid });

                        db.SaveChanges();
                    }
                }

                ViewBag.Message = "Upload successful";
                return RedirectToAction("Edit/" + TempData["TicketGuid"].ToString());
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }

        // GET: AdminTickets/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            service_tickets service_tickets = db.service_tickets.Find(id);
            if (service_tickets == null)
            {
                return HttpNotFound();
            }
            return View(service_tickets);
        }
        public ActionResult Download(Guid? id)
        {
            var fileNameGuid = id.ToString();
            Guid myFileGuid = Guid.Parse(fileNameGuid);

            var found = from s in db.service_ticket_files
                        where s.guid.Equals(myFileGuid)
                        select s;
            var x = found.Count();
            var myFileName = found.First().file_name;
            var myFileExt = found.First().file_ext;
            var myPath = Path.Combine(Server.MapPath("~/Content/service_ticket_uploads"), myFileGuid.ToString("B") + myFileExt);
            byte[] fileBytes = System.IO.File.ReadAllBytes(@myPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, myFileName);
        }
        // GET: AdminTickets/Create
        public ActionResult Create()
        {
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name");
            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true).OrderBy(o => o.list_num), "ID", "Name");
            ViewBag.CategoryID = new SelectList(db.ServiceCategories.Where(o => o.active == true).OrderBy(o => o.Name), "ID", "Name");
            ViewBag.CompanyID = new SelectList(db.Companies.Where(o => o.active == true).OrderBy(o =>o.name), "guid", "name");
            ViewBag.LocationDD = new SelectList(db.locationinfoes.Where(o => o.active == true).OrderBy(o => o.name), "guid", "name");
            ViewBag.FullName = Helpers.UserHelper.getUserName();
            var myphones = Helpers.UserHelper.getUserPhones();
            string[] phones = myphones.Split('#');
            ViewBag.DayPhone = phones[0];
            ViewBag.NightPhone = phones[1];            
            return View();
        }

        // POST: AdminTickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "parent_company_guid,service_location_guid,problem_summary,problem_details,location_contact_name,location_contact_phone,location_contact_phone_night,cost_code,EQmodel,EQserial,EQProbDesc,service_provider,internal_notes,cust_po_num,CategoryID,PriorityID")] service_tickets service_tickets)
        {
            var mySvcCat = Request.Form["SvcCat"];
            var myPriStatus = Request.Form["PriStatus"];
            var strLocationGuid = Request.Form["LocationDD"];
            var mylocguid = Guid.Parse(strLocationGuid);
            var found = db.locationinfoes.FirstOrDefault(u => u.guid.Equals(mylocguid));
            var ParentLoactionGuid = found.parentguid.ToString();
            Guid mypappy = Guid.Parse(ParentLoactionGuid);
            var LocationName = found.name;
            var LocationRegion = found.region;
            var LocationCity = found.city;
            var LastUserGUID = Helpers.UserHelper.getUserGuid();
            int maxNumID = Helpers.SvcHelper.getSvcIDMax() + 1;
            int maxNumST = Helpers.SvcHelper.getjobNumMax() + 1;


            if (ModelState.IsValid)
            {
                service_tickets.id = maxNumID;
                service_tickets.job_number = maxNumST;
                service_tickets.created_by_user_guid = LastUserGUID;
                service_tickets.last_updated_by_user_guid = LastUserGUID;
                service_tickets.parent_company_guid = mypappy;
                service_tickets.service_location_guid = mylocguid;
                service_tickets.last_update_datetime = DateTime.Now;
                service_tickets.order_datetime = DateTime.Now;
                service_tickets.ServiceCategory = mySvcCat;
                service_tickets.PriorityStatus = myPriStatus;
                service_tickets.StatusID = 9;
                service_tickets.StatusName = "Open";
                service_tickets.active = true;
                service_tickets.Location = LocationName;
                service_tickets.Region = LocationRegion;
                service_tickets.City = LocationCity;
                service_tickets.guid = Guid.NewGuid();
                db.service_tickets.Add(service_tickets);
                db.SaveChanges();

                try
                {
                    var email = new Correspondence.Mail();
                    var user = db.AspNetUsers.Where(o => o.guid == service_tickets.last_updated_by_user_guid).ToList()[0];

                    var body = "AAA Web Portal Service Ticket\r\n\r\n" +
                               "Requested By: " + user.fname.ToUpper() + " " + user.lname.ToUpper() + "\r\n" +
                               "Customer Number " + found.cf_location_num + "\r\n" +
                               "Cost Code " + service_tickets.cost_code + "\r\n" +
                               "Customer PO# " + service_tickets.cust_po_num + "\r\n" +
                               "Service Provider " + service_tickets.service_provider + "\r\n" +
                               "Service Location: " + service_tickets.Location.ToUpper() + "\r\n" +
                               "Address Line 1: " + (found.addressline1 != null ? found.addressline1.ToUpper() : "") + "\r\n" +
                               "Address Line 2: " + (found.addressline2 != null ? found.addressline2.ToUpper() : "") + "\r\n" +
                               "City: " + (found.city != null ? found.city.ToUpper() : "") + "\r\n" +
                               "State: " + (found.state != null ? found.state.ToUpper() : "") + "\r\n" +
                               "Zip: " + (found.zip > 0 ? found.zip.ToString() : "") + "\r\n" +
                               "Job Number: " + service_tickets.job_number + "\r\n" +
                               "Contact Name: " + found.name.ToUpper() + "\r\n" +
                               "Contact Number: " + service_tickets.location_contact_phone + "\r\n" +
                               "Contact After Hours Number: " + service_tickets.location_contact_phone_night + "\r\n" +
                               "Priority Code: " + service_tickets.PriorityID + " - " + db.PriorityLists.Where(o => o.ID == service_tickets.PriorityID).ToList()[0].Name.ToUpper() + "\r\n" +
                               "Priority Code: " + service_tickets.PriorityID + "\r\n" +
                               "Order Date: " + service_tickets.order_datetime.ToShortDateString() + "\r\n" +
                               "Order Time: " + service_tickets.order_datetime.ToShortTimeString() + "\r\n" +
                               "Category: " + service_tickets.ServiceCategory + "\r\n" +
                               "Request Summary\r\n" +
                               service_tickets.problem_summary.ToUpper() + "\r\n" +
                               "Request Details\r\n" +
                               service_tickets.problem_details.ToUpper() + "\r\n" +
                               "Status Code: " + service_tickets.StatusName.ToUpper() + "\r\n" +

                               //I checked with our reps.They never used Zone or service rep function in old portal.  We can drop it off and make the list small.
                               //"Zone: " + "WHERE DOES THIS VALUE COME FROM?" + "\r\n" +
                               //"Service Type: " + service_tickets.ServiceCategory.ToUpper() + "\r\n" +

                               //"Service Rep: " + "WHERE DOES THIS VALUE COME FROM?" + "\r\n" +
                               "Taken By: Web Portal\r\n\r\n" +
                               "If you have questions or concerns about this message please contact us at 1-800-892-4784.\r\n\r\n" +
                               "Please do not reply to this e-mail, this account is not monitored.";

                    email.Send(subject: "Web Portal Service Ticket Entered", body: body, email: user.Email, location: mylocguid);
                }
                catch (Exception e)
                {
                    System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                }

                return RedirectToAction("Index");
            }

            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", service_tickets.StatusID);
            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true).OrderBy(o => o.list_num), "ID", "Name", service_tickets.PriorityID);            
            ViewBag.CategoryID = new SelectList(db.ServiceCategories.Where(o => o.active == true).OrderBy(o => o.Name), "ID", "Name", service_tickets.CategoryID);
            ViewBag.CompanyID = new SelectList(db.Companies.Where(o => o.active == true).OrderBy(o => o.name), "guid", "name", service_tickets.parent_company_guid);
            ViewBag.LocationDD = new SelectList(db.locationinfoes.Where(o => o.active == true).OrderBy(o => o.name), "guid", "name", service_tickets.service_location_guid);
            return View(service_tickets);
        }

        // GET: AdminTickets/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            service_tickets service_tickets = db.service_tickets.Find(id);
            ViewBag.CreatedBy = Helpers.SvcHelper.getCreatedby(service_tickets.created_by_user_guid);
            TempData["TicketGuid"] = id;
            ViewBag.TicketNumber = service_tickets.job_number;
            ViewBag.StatusName = service_tickets.StatusName;
            ViewBag.LocationName = service_tickets.Location;

            if (service_tickets == null)
            {
                return HttpNotFound();
            }

            var gparentGuid = service_tickets.parent_company_guid;
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", service_tickets.StatusID);
            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true).OrderBy(o => o.list_num), "ID", "Name", service_tickets.PriorityID);            
            ViewBag.CategoryID = new SelectList(db.ServiceCategories.Where(o => o.active == true).OrderBy(o => o.Name), "ID", "Name", service_tickets.CategoryID);
            ViewBag.LocationDD = new SelectList(db.locationinfoes.Where(o => o.parentguid == gparentGuid).Where(s => s.active == true).OrderBy( s => s.name), "guid", "name", service_tickets.service_location_guid);

            ViewBag.FileList = from s in db.service_ticket_files
                               where s.ticket_guid == id
                               orderby s.date_in descending
                               select s;

            //return View(service_tickets);
            var tupleAlternative = new TupleAlternative();            
            tupleAlternative.ServiceTickets = service_tickets;
            var locationInfo = db.locationinfoes.Where(o => o.active == true && o.guid == service_tickets.service_location_guid);
            tupleAlternative.LocationInfo = locationInfo.ToList()[0];
            return View(tupleAlternative);
        }

        // POST: AdminTickets/Edit/5
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

            var strLocationGuid = Request.Form["LocationDD"];
            var mylocguid = Guid.Parse(strLocationGuid);
            var found = db.locationinfoes.FirstOrDefault(u => u.guid.Equals(mylocguid));
            var myCity = found.city;
            var myLocxName = found.name;
            var myLocxRegion = found.region;
            var ticketToUpdate = db.service_tickets.Find(id);
            var lastuserguid = Helpers.UserHelper.getUserGuid();
            var mytime = DateTime.Now.ToString();
            var myNotes = Request.Form["ProbSum"];
            var myNotes2 = "Updated " + mytime + System.Environment.NewLine + myNotes;
            var myProblems = Request.Form["ServiceTickets.problem_details"];
            string myNotes3 = "";

            if (myNotes != "")
            {
                myNotes3 = myProblems + System.Environment.NewLine + System.Environment.NewLine + myNotes2;
            }
            else
            {
                myNotes3 = myProblems;
            }

            //var myStatus = Request.Form["mStatusName"];            
            var myVNotes = Request.Form["ResNotes"];
            var myVNotes2 = "Updated " + mytime + System.Environment.NewLine + myVNotes;
            var myVProblems = Request.Form["ServiceTickets.notes"];
            var internalNotes = Request.Form["ServiceTickets.internal_notes"];
            string myVNotes3 = "";

            if (myVNotes != "")
            {
                myVNotes3 = myVProblems + System.Environment.NewLine + System.Environment.NewLine + myVNotes2;
            }
            else
            {
                myVNotes3 = myVProblems;
            }

            ticketToUpdate.PriorityID = Request.Form["PriorityID"] != null ? Request.Form["PriorityID"] != "" ? int.Parse(Request.Form["PriorityID"]) : ticketToUpdate.PriorityID : ticketToUpdate.PriorityID;

            ticketToUpdate.PriorityStatus = db.PriorityLists.Where(o => o.ID == ticketToUpdate.PriorityID).Select(o => o.Name).ToList()[0];

            ticketToUpdate.CategoryID = Request.Form["CategoryID"] != null ? Request.Form["CategoryID"] != "" ? int.Parse(Request.Form["CategoryID"]) : ticketToUpdate.CategoryID : ticketToUpdate.CategoryID;

            ticketToUpdate.ServiceCategory = db.ServiceCategories.Where(o => o.ID == ticketToUpdate.CategoryID).Select(o => o.Name).ToList()[0];

            if (TryUpdateModel(ticketToUpdate, "",
            new string[] { "service_location_guid", "StatusID", "complete_datetime", "service_provider", "cost_code", "cust_po_num", "total_billing", "location_contact_name", "accepted_datetime", "dispatch_datetime", "problem_details", "location_contact_phone", "location_contact_phone_night", "closed_datetime", "notes", "active", "internal_notes" }))
            {
                try
                {
                    var myStatus = db.StatusLists.Where(sl => sl.ID == ticketToUpdate.StatusID).ToList()[0].Name;

                    ticketToUpdate.service_provider = Request.Form["ServiceTickets.service_provider"];
                    ticketToUpdate.cost_code = Request.Form["ServiceTickets.cost_code"];
                    ticketToUpdate.cust_po_num = Request.Form["ServiceTickets.cust_po_num"];
                    ticketToUpdate.total_billing = Request.Form["ServiceTickets.total_billing"] != "" ? int.Parse(Request.Form["ServiceTickets.total_billing"]) : 0;
                    ticketToUpdate.location_contact_name = Request.Form["ServiceTickets.location_contact_name"];
                    ticketToUpdate.location_contact_phone = Request.Form["ServiceTickets.location_contact_phone"];
                    ticketToUpdate.location_contact_phone_night = Request.Form["ServiceTickets.location_contact_phone_night"];
                    ticketToUpdate.problem_details = myNotes3;
                    ticketToUpdate.StatusName = myStatus;
                    ticketToUpdate.service_location_guid = mylocguid;
                    ticketToUpdate.Location = myLocxName;
                    ticketToUpdate.City = myCity;
                    ticketToUpdate.Region = myLocxRegion;
                    ticketToUpdate.notes = myVNotes3;
                    ticketToUpdate.internal_notes = internalNotes;

                    if (ticketToUpdate.CategoryID == 19 || ticketToUpdate.CategoryID == 21 || ticketToUpdate.CategoryID == 22)
                    {
                        ticketToUpdate.EQmodel = Request.Form["ServiceTickets.EQmodel"];
                        ticketToUpdate.EQserial = Request.Form["ServiceTickets.EQserial"];
                        ticketToUpdate.EQProbDesc = Request.Form["ServiceTickets.EQProbDesc"];
                    }
                    else
                    {
                        ticketToUpdate.EQmodel = "";
                        ticketToUpdate.EQserial = "";
                        ticketToUpdate.EQProbDesc = "";
                    }

                    if (Request.Form["ServiceTickets.accepted_datetime"] != "")
                    {
                        ticketToUpdate.accepted_datetime =  DateTime.Parse(Request.Form["ServiceTickets.accepted_datetime"]);
                    }

                    if (Request.Form["ServiceTickets.dispatch_datetime"] != "")
                    {
                        ticketToUpdate.dispatch_datetime = DateTime.Parse(Request.Form["ServiceTickets.dispatch_datetime"]);
                    }

                    if (Request.Form["ServiceTickets.complete_datetime"] != "")
                    {
                        ticketToUpdate.complete_datetime = DateTime.Parse(Request.Form["ServiceTickets.complete_datetime"]);
                    }

                    if (Request.Form["ServiceTickets.closed_datetime"] != "")
                    {
                        ticketToUpdate.closed_datetime = DateTime.Parse(Request.Form["ServiceTickets.closed_datetime"]);
                    }

                    ticketToUpdate.last_update_datetime = DateTime.Now;
                    ticketToUpdate.last_updated_by_user_guid = lastuserguid;
                    db.SaveChanges();

                    try
                    {
                        var email = new Correspondence.Mail();
                        var user = db.AspNetUsers.Where(o => o.guid == ticketToUpdate.last_updated_by_user_guid).ToList()[0];

                        var body = "AAA Web Portal Service Ticket\r\n\r\n" +
                                   "The Service Request number: " + ticketToUpdate.job_number + " for location " + db.locationinfoes.Where(o => o.guid == ticketToUpdate.service_location_guid).ToList()[0].name.ToUpper() + " has been updated." + "\r\n" +
                                   "The request is now: " + ticketToUpdate.StatusName.ToUpper() + "\r\n" +
                                   "The Requests Details follow.\r\n" +
                                   "Requested: " + (ticketToUpdate.order_datetime != null ? ticketToUpdate.order_datetime.ToString("M/d/yyyy hh:mm:ss tt") : "") + "\r\n" +
                                   "Accepted: " + (ticketToUpdate.accepted_datetime != null ? ticketToUpdate.accepted_datetime?.ToString("M/d/yyyy hh:mm:ss tt") : "") + "\r\n" +
                                   "Dispatched: " + (ticketToUpdate.dispatch_datetime != null ? ticketToUpdate.dispatch_datetime?.ToString("M/d/yyyy hh:mm:ss tt") : "") + "\r\n" +
                                   "Completed: " + (ticketToUpdate.complete_datetime != null ? ticketToUpdate.complete_datetime?.ToString("M/d/yyyy hh:mm:ss tt") : "") + "\r\n" +
                                   "Service Provider: " + (ticketToUpdate.service_provider != null ? ticketToUpdate.service_provider.ToUpper() : "") + "\r\n" +
                                   "Problem Summary: " + ticketToUpdate.problem_summary.ToUpper() + "\r\n" +
                                   "Problem Details: " + ticketToUpdate.problem_details.ToUpper() + "\r\n" +
                                   "Resolution Notes: " + (ticketToUpdate.notes != null ? ticketToUpdate.notes.ToUpper() : "") + "\r\n" +
                                   "Updated By: " + user.fname.ToUpper() + " " + user.lname.ToUpper() + "\r\n" +
                                   "Taken By: Web Portal\r\n\r\n" +
                                   "If you have questions or concerns about this message please contact us at 1-800-892-4784.\r\n\r\n" +
                                   "Please do not reply to this e-mail, this account is not monitored.";

                        email.Send(subject: "Web Portal Service Ticket Update", body: body, email: user.Email, location: mylocguid);
                    }
                    catch (Exception e)
                    {
                        System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                    }

                    return RedirectToAction("Index");
                }
                //catch (DataException /* dex */)
                catch (DataException dex)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator." + dex.Message);
                }
            }

            var gparentGuid = ticketToUpdate.parent_company_guid;
            ViewBag.StatusID = new SelectList(db.StatusLists, "ID", "Name", ticketToUpdate.StatusID);
            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true).OrderBy(o => o.list_num), "ID", "Name", ticketToUpdate.PriorityID);            
            ViewBag.CategoryID = new SelectList(db.ServiceCategories.Where(o => o.active == true).OrderBy(o => o.Name), "ID", "Name", ticketToUpdate.CategoryID);
            ViewBag.LocationDD = new SelectList(db.locationinfoes.Where(o => o.parentguid == gparentGuid).Where(s => s.active == true).OrderBy(s => s.name), "guid", "name", ticketToUpdate.service_location_guid);
            //return View(ticketToUpdate);
            var locationInfo = db.locationinfoes.Where(o => o.active == true && o.guid == ticketToUpdate.service_location_guid);
            return View(new TupleAlternative { ServiceTickets = ticketToUpdate, LocationInfo = locationInfo.ToList()[0] });
        }

        // GET: AdminTickets/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            service_tickets service_tickets = db.service_tickets.Find(id);
            if (service_tickets == null)
            {
                return HttpNotFound();
            }
            return View(service_tickets);
        }

        // POST: AdminTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            service_tickets service_tickets = db.service_tickets.Find(id);
            db.service_tickets.Remove(service_tickets);
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

        public ActionResult SetServiceLocation(string locationGUID)
        {
            TempData["LocationGuid"] = locationGUID;

            if (locationGUID != "0")
            {
                var LocationGUID = Guid.Parse(locationGUID);
                var locationsInfo = db.locationinfoes.Where(o => o.guid == LocationGUID).ToList()[0];
                var result = new { locationsInfo.addressline1, locationsInfo.addressline2, locationsInfo.city, locationsInfo.zip };
                return Json(result);
            }
            else
            {
                return null;
            }
        }
    }
}