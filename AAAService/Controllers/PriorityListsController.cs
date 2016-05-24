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
    public class PriorityListsController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: PriorityLists
        public ActionResult Index()
        {
            return View(db.PriorityLists.ToList());
        }

        // GET: PriorityLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PriorityList priorityList = db.PriorityLists.Find(id);
            if (priorityList == null)
            {
                return HttpNotFound();
            }
            return View(priorityList);
        }

        // GET: PriorityLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PriorityLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,code,list_num,active")] PriorityList priorityList)
        {
            if (ModelState.IsValid)
            {
                db.PriorityLists.Add(priorityList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(priorityList);
        }

        // GET: PriorityLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PriorityList priorityList = db.PriorityLists.Find(id);
            if (priorityList == null)
            {
                return HttpNotFound();
            }
            return View(priorityList);
        }

        // POST: PriorityLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,code,list_num,active")] PriorityList priorityList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(priorityList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(priorityList);
        }

        // GET: PriorityLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PriorityList priorityList = db.PriorityLists.Find(id);
            if (priorityList == null)
            {
                return HttpNotFound();
            }
            return View(priorityList);
        }

        // POST: PriorityLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PriorityList priorityList = db.PriorityLists.Find(id);
            db.PriorityLists.Remove(priorityList);
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
