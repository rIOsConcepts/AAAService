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
    public class UserLocationController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: UserLocation
        public ActionResult Index()
        {
            var user_to_location = db.user_to_location.Include(u => u.locationinfo);            
            return View(user_to_location.ToList());
        }

        // GET: UserLocation/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_to_location user_to_location = db.user_to_location.Find(id);
            if (user_to_location == null)
            {
                return HttpNotFound();
            }
            return View(user_to_location);
        }

        // GET: UserLocation/Create
        public ActionResult Create()
        {
            ViewBag.location_guid = new SelectList(db.locationinfoes, "guid", "name");            
            return View();
        }
        
        // POST: UserLocation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "guid,user_guid,location_guid,primary_contact_bit")] user_to_location user_to_location)
        {
            if (ModelState.IsValid)
            {
                user_to_location.guid = Guid.NewGuid();
                db.user_to_location.Add(user_to_location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.location_guid = new SelectList(db.locationinfoes, "guid", "name", user_to_location.location_guid);
            return View(user_to_location);
        }

        // GET: UserLocation/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_to_location user_to_location = db.user_to_location.Find(id);
            if (user_to_location == null)
            {
                return HttpNotFound();
            }
            ViewBag.location_guid = new SelectList(db.locationinfoes, "guid", "name", user_to_location.location_guid);
            return View(user_to_location);
        }

        // POST: UserLocation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "guid,user_guid,location_guid,primary_contact_bit")] user_to_location user_to_location)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_to_location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.location_guid = new SelectList(db.locationinfoes, "guid", "name", user_to_location.location_guid);
            return View(user_to_location);
        }

        // GET: UserLocation/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_to_location user_to_location = db.user_to_location.Find(id);
            if (user_to_location == null)
            {
                return HttpNotFound();
            }
            return View(user_to_location);
        }

        // POST: UserLocation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            user_to_location user_to_location = db.user_to_location.Find(id);
            db.user_to_location.Remove(user_to_location);
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
