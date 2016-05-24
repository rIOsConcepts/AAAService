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
{
    [Authorize(Roles = "SiteAdmin")]
    public class locationinfoesController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: locationinfoes
        public ActionResult Index()
        {
            var locationinfoes = db.locationinfoes.Include(l => l.Company).Include(l => l.Location_Service_Provider);
            return View(locationinfoes.ToList());
        }

        // GET: locationinfoes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            locationinfo locationinfo = db.locationinfoes.Find(id);
            if (locationinfo == null)
            {
                return HttpNotFound();
            }
            return View(locationinfo);
        }

        // GET: locationinfoes/Create
        public ActionResult Create()
        {
            ViewBag.parentguid = new SelectList(db.Companies, "guid", "name");
            ViewBag.guid = new SelectList(db.Location_Service_Provider, "guid", "guid");
            return View();
        }

        // POST: locationinfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "guid,name,addressline1,addressline2,city,state,zip,parentguid,cf_location_num,cf_company_num,region,email_all_members,active,RegionID")] locationinfo locationinfo)
        {
            if (ModelState.IsValid)
            {
                locationinfo.guid = Guid.NewGuid();
                db.locationinfoes.Add(locationinfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.parentguid = new SelectList(db.Companies, "guid", "name", locationinfo.parentguid);
            ViewBag.guid = new SelectList(db.Location_Service_Provider, "guid", "guid", locationinfo.guid);
            return View(locationinfo);
        }

        // GET: locationinfoes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            locationinfo locationinfo = db.locationinfoes.Find(id);
            if (locationinfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.parentguid = new SelectList(db.Companies, "guid", "name", locationinfo.parentguid);
            ViewBag.guid = new SelectList(db.Location_Service_Provider, "guid", "guid", locationinfo.guid);
            return View(locationinfo);
        }

        // POST: locationinfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "guid,name,addressline1,addressline2,city,state,zip,parentguid,cf_location_num,cf_company_num,region,email_all_members,active,RegionID")] locationinfo locationinfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(locationinfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.parentguid = new SelectList(db.Companies, "guid", "name", locationinfo.parentguid);
            ViewBag.guid = new SelectList(db.Location_Service_Provider, "guid", "guid", locationinfo.guid);
            return View(locationinfo);
        }

        // GET: locationinfoes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            locationinfo locationinfo = db.locationinfoes.Find(id);
            if (locationinfo == null)
            {
                return HttpNotFound();
            }
            return View(locationinfo);
        }

        // POST: locationinfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            locationinfo locationinfo = db.locationinfoes.Find(id);
            db.locationinfoes.Remove(locationinfo);
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
