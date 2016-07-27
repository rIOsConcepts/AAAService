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
                return RedirectToAction("Index");
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
            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name");
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name");
            ViewBag.CategoryID = new SelectList(db.ServiceCategories, "ID", "Name");
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
                return RedirectToAction("Index");
            }

            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name", service_tickets.PriorityID);
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", service_tickets.StatusID);
            ViewBag.CategoryID = new SelectList(db.ServiceCategories, "ID", "Name", service_tickets.CategoryID);
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
            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name", service_tickets.PriorityID);
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", service_tickets.StatusID);
            ViewBag.CategoryID = new SelectList(db.ServiceCategories, "ID", "Name", service_tickets.CategoryID);
            ViewBag.LocationDD = new SelectList(db.locationinfoes.Where(o => o.parentguid == gparentGuid).Where(s => s.active == true).OrderBy( s => s.name), "guid", "name", service_tickets.service_location_guid);
            ViewBag.FileList = from s in db.service_ticket_files
                               where s.ticket_guid == id
                               select s;
            return View(service_tickets);
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
            var myProblems = Request.Form["problem_details"];
            string myNotes3 = "";
            if (myNotes != "")
            {
                myNotes3 = myProblems + System.Environment.NewLine + System.Environment.NewLine + myNotes2;
            }
            else
            {
                myNotes3 = myProblems;
            }
            var myStatus = Request.Form["mStatusName"];
            var myVNotes = Request.Form["ResNotes"];
            var myVNotes2 = "Updated " + mytime + System.Environment.NewLine + myVNotes;
            var myVProblems = Request.Form["notes"];
            string myVNotes3 = "";
            if (myVNotes != "")
            {
                myVNotes3 = myVProblems + System.Environment.NewLine + System.Environment.NewLine + myVNotes2;
            }
            else
            {
                myVNotes3 = myVProblems;
            }
            if (TryUpdateModel(ticketToUpdate, "",
            new string[] { "service_location_guid", "StatusID", "complete_datetime", "service_provider", "cost_code", "cust_po_num", "total_billing", "location_contact_name", "accepted_datetime", "dispatch_datetime", "problem_details", "location_contact_phone", "location_contact_phone_night", "closed_datetime", "notes", "active", "internal_notes" }))
            {
                try
                {
                    ticketToUpdate.problem_details = myNotes3;
                    ticketToUpdate.StatusName = myStatus;
                    ticketToUpdate.service_location_guid = mylocguid;
                    ticketToUpdate.Location = myLocxName;
                    ticketToUpdate.City = myCity;
                    ticketToUpdate.Region = myLocxRegion;
                    ticketToUpdate.notes = myVNotes3;
                    ticketToUpdate.last_update_datetime = DateTime.Now;
                    ticketToUpdate.last_updated_by_user_guid = lastuserguid;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            var gparentGuid = ticketToUpdate.parent_company_guid;

            ViewBag.PriorityID = new SelectList(db.PriorityLists, "ID", "Name", ticketToUpdate.PriorityID);
            ViewBag.StatusID = new SelectList(db.StatusLists, "ID", "Name", ticketToUpdate.StatusID);
            ViewBag.CategoryID = new SelectList(db.ServiceCategories, "ID", "Name", ticketToUpdate.CategoryID);
            ViewBag.LocationDD = new SelectList(db.locationinfoes.Where(o => o.parentguid == gparentGuid).Where(s => s.active == true).OrderBy(s => s.name), "guid", "name", ticketToUpdate.service_location_guid);
            return View(ticketToUpdate);
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
    }
}
