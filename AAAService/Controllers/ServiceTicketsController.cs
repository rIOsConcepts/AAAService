using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AAAService.Models;
using AAAService.Helpers;

namespace AAAService.Controllers
{
    [Authorize]
    public class ServiceTicketsController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: ServiceTickets
        public ActionResult Index()
        {
            var service_tickets = db.service_tickets.Include(s => s.PriorityList).Include(s => s.ServiceCategory1).Include(s => s.StatusList);
            return View();
        }

        //Action result for ajax call
        [HttpPost]
        public ActionResult GetServiceLocation(string companyGUID)
        {
            SelectList obg = new SelectList(db.locationinfoes.Where(o => o.parentguid.ToString() == companyGUID && o.active == true).OrderBy(o => o.name), "guid", "Name");
            return Json(obg);
        }
        public ActionResult SetServiceLocation(string locationGUID)
        {
            TempData["LocationGuid"] = locationGUID;
            return null;
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
            var myNewGuid = Guid.NewGuid();
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
        // GET: ServiceTickets/Details/5
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
        // GET: ServiceTickets/Create
        public ActionResult Create(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var myguid = id;

            TempData["LocationGuid"] = id;
            //var myloc = Helpers.SvcHelper.getParentGuid(id);
            // TempData["ParentLocation"] = ;
            ViewBag.FullName = Helpers.UserHelper.getUserName();
            var myphones = Helpers.UserHelper.getUserPhones();
            string[] phones = myphones.Split('#');
            ViewBag.DayPhone = phones[0];

            if (phones.Count() > 1)
            {
                ViewBag.NightPhone = phones[1];
            }

            var companyGUID = db.locationinfoes.Where(o => o.active == true && o.guid.ToString() == id.ToString()).ToList()[0].parentguid;
            //var companyName = db.Companies.Where(o => o.active == true && o.guid.ToString() == companyGUID).ToList()[0].name;
            var companyList = CompanyList(companyGUID);
            ViewBag.CompanyList = companyList;
            //TempData["CompanyGUID"] = companyGUID;
            ViewBag.CompanyGUID = companyGUID;
            //ViewBag.LocationName = db.locationinfoes.Where(o => o.active == true && o.guid.ToString() == id.ToString()).ToList()[0].name;
            ViewBag.LocationName = id;
            //ViewBag.LocationDD = new SelectList(db.locationinfoes.Where(o => o.active == true).OrderBy(o => o.name), "guid", "name", service_tickets.service_location_guid);            
            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true).OrderBy(o => o.list_num), "ID", "Name");
            ViewBag.CategoryID = new SelectList(db.ServiceCategories.Where(o => o.active == true), "ID", "Name");
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name");
            return View();
            //return View(new Tuple<AAAService.Models.CountryModel, AAAService.Models.service_tickets>(objcountrymodel, new service_tickets()));
        }

        internal List<SelectListItem> CompanyList(Guid? companyGUID)
        {
            var selectList = new SelectList(db.Companies.Where(o => o.active == true), "guid", "Name").OrderBy(o => o.Text).ToList();

            foreach (var item in selectList)
            {
                if (item.Value == companyGUID.ToString())
                {
                    item.Selected = true;
                    break;
                }
            }

            return selectList;
        }

        // POST: ServiceTickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "problem_summary,problem_details,CompanyID,LocationID,location_contact_name,location_contact_phone,location_contact_phone_night,cost_code,EQmodel,EQserial,EQProbDesc,service_provider,cust_po_num,CategoryID,Region,PriorityID,StatusName,StatusID,Location,City")] service_tickets service_tickets)
        {
            var id = TempData.Peek("LocationGuid");
            Guid locationGUID = Guid.Parse(id.ToString());
            var mySvcCat = Request.Form["SvcCat"];
            var myPriStatus = Request.Form["PriStatus"];
            var myPriorityID = string.IsNullOrEmpty(Request.Form["PriorityID"]) ? 0 : int.Parse(Request.Form["PriorityID"]);


            var found = db.locationinfoes.FirstOrDefault(u => u.guid.Equals(locationGUID));
            var LocationName = found.name;
            var ParentLoactionGuid = found.parentguid.ToString();
            Guid parentGUID = Guid.Parse(ParentLoactionGuid);
            var LocationRegion = found.region;
            var LocationCity = found.city;
            var LastUserGUID = Helpers.UserHelper.getUserGuid();
            int maxNumID = Helpers.SvcHelper.getSvcIDMax() + 1;
            int maxNumST = Helpers.SvcHelper.getjobNumMax() + 1;

            // var errors = ModelState.Values.SelectMany(v => v.Errors);*/
            if (ModelState.IsValid)
            {

                service_tickets.id = maxNumID;
                service_tickets.job_number = maxNumST;
                service_tickets.created_by_user_guid = LastUserGUID;
                service_tickets.parent_company_guid = parentGUID;
                service_tickets.service_location_guid = locationGUID;
                service_tickets.last_updated_by_user_guid = LastUserGUID;
                service_tickets.last_update_datetime = DateTime.Now;
                service_tickets.order_datetime = DateTime.Now;
                service_tickets.ServiceCategory = mySvcCat;
                //service_tickets.PriorityStatus = myPriStatus;
                service_tickets.PriorityStatus = db.PriorityLists.Where(o => o.active == true && o.ID == myPriorityID).ToList()[0].Name;
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
                               "Address Line 1: " + found.addressline1.ToUpper() + "\r\n" +
                               "Address Line 2: " + found.addressline2.ToUpper() + "\r\n" +
                               "City: " + found.city.ToUpper() + "\r\n" +
                               "State: " + found.state.ToUpper() + "\r\n" +
                               "Zip: " + found.zip + "\r\n" +
                               "Job Number: " + service_tickets.job_number + "\r\n" +
                               "Contact Name: " + found.name.ToUpper() + "\r\n" +
                               "Contact Number: " + service_tickets.location_contact_phone + "\r\n" +
                               "Contact After Hours Number: " + service_tickets.location_contact_phone_night + "\r\n" +
                               "Priority Code: " + service_tickets.PriorityID + " - " + db.PriorityLists.Where(o => o.ID == service_tickets.PriorityID).ToList()[0].Name.ToUpper() + "\r\n" +
                               "Priority Code: " + service_tickets.PriorityID + "\r\n" +
                               "Order Date: " + service_tickets.order_datetime.ToShortDateString() + "\r\n" +
                               "Order Time: " + service_tickets.order_datetime.ToShortTimeString() + "\r\n" +
                               "Category " + service_tickets.CategoryID + "\r\n" +
                               "Request Summary\r\n" +
                               service_tickets.problem_summary.ToUpper() + "\r\n" +
                               "Request Details\r\n" +
                               service_tickets.problem_details.ToUpper() + "\r\n" +
                               "Status Code: " + service_tickets.StatusName.ToUpper() + "\r\n" +
                               "Zone: " + "WHERE DOES THIS VALUE COME FROM?" + "\r\n" +
                               "Service Type: " + service_tickets.ServiceCategory.ToUpper() + "\r\n" +
                               "Service Rep: " + "WHERE DOES THIS VALUE COME FROM?" + "\r\n" +
                               "Taken By: Web Portal\r\n\r\n" +
                               "If you have questions or concerns about this message please contact us at 1-800-892-4784.\r\n\r\n" +
                               "Please do not reply to this e - mail, this account is not monitored.";

                    email.Send("Web Portal Service Ticket Entered", body);
                }
                catch (Exception e)
                {
                    System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                }

                return RedirectToAction("Index", "Home");
            }

            var companyList = CompanyList(parentGUID);
            ViewBag.CompanyList = companyList;
            ViewBag.CompanyGUID = parentGUID;
            ViewBag.LocationName = locationGUID;
            ViewBag.FullName = service_tickets.location_contact_name;
            ViewBag.DayPhone = service_tickets.location_contact_phone;
            ViewBag.NightPhone = service_tickets.location_contact_phone_night;
            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true), "ID", "Name", service_tickets.PriorityStatus);
            ViewBag.CategoryID = new SelectList(db.ServiceCategories.Where(o => o.active == true), "ID", "Name", service_tickets.ServiceCategory);
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", service_tickets.StatusName);
            return View(service_tickets);
        }

        // GET: ServiceTickets/Edit/5
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

            if (service_tickets == null)
            {
                return HttpNotFound();
            }

            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true), "ID", "Name", service_tickets.PriorityID);
            ViewBag.CategoryID = new SelectList(db.ServiceCategories.Where(o => o.active == true), "ID", "Name", service_tickets.CategoryID);
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", service_tickets.StatusID);

            ViewBag.FileList = from s in db.service_ticket_files
                               where s.ticket_guid == id
                               select s;            

            return View(service_tickets);
        }

        // POST: ServiceTickets/Edit/5
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

            var ticketToUpdate = db.service_tickets.Find(id);
            var lastuserguid = Helpers.UserHelper.getUserGuid();
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

                //MR Removed validation below because it fails every time
                //if (TryUpdateModel(ticketToUpdate, "", new string[] { "problem_details", "location_contact_phone", "location_contact_phone_night", "internal_notes" }))
                //{
                    try
                    {
                        ticketToUpdate.problem_details = problem_details;
                        ticketToUpdate.internal_notes = internalNotes;
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
                                       "Please do not reply to this e - mail, this account is not monitored.";

                            email.Send("Web Portal Service Ticket Update", body);
                        }
                        catch (Exception e)
                        {
                            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                        }

                    return RedirectToAction("Index", "Home");
                    }
                    catch (DataException /*dex*/)
                    {
                        //Log the error (uncomment dex variable name and add a line here to write a log.
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                //}
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PriorityID = new SelectList(db.PriorityLists.Where(o => o.active == true), "ID", "Name", ticketToUpdate.PriorityID);
            ViewBag.CategoryID = new SelectList(db.ServiceCategories.Where(o => o.active == true), "ID", "Name", ticketToUpdate.CategoryID);
            ViewBag.StatusID = new SelectList(db.StatusLists.Where(o => o.active == true), "ID", "Name", ticketToUpdate.StatusID);
            return View(ticketToUpdate);
        }

        [Authorize(Roles = "SiteAdmin")]
        // GET: ServiceTickets/Delete/5
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
        [Authorize(Roles = "SiteAdmin")]
        // POST: ServiceTickets/Delete/5
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
