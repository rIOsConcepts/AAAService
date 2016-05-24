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
    public class CommentRatingsController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: CommentRatings
        public ActionResult Index()
        {
            return View(db.CommentRatings.ToList());
        }

        // GET: CommentRatings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentRating commentRating = db.CommentRatings.Find(id);
            if (commentRating == null)
            {
                return HttpNotFound();
            }
            return View(commentRating);
        }

        // GET: CommentRatings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CommentRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RatingID,Name,RatingValue")] CommentRating commentRating)
        {
            if (ModelState.IsValid)
            {
                db.CommentRatings.Add(commentRating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(commentRating);
        }

        // GET: CommentRatings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentRating commentRating = db.CommentRatings.Find(id);
            if (commentRating == null)
            {
                return HttpNotFound();
            }
            return View(commentRating);
        }

        // POST: CommentRatings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RatingID,Name,RatingValue")] CommentRating commentRating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commentRating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(commentRating);
        }

        // GET: CommentRatings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentRating commentRating = db.CommentRatings.Find(id);
            if (commentRating == null)
            {
                return HttpNotFound();
            }
            return View(commentRating);
        }

        // POST: CommentRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommentRating commentRating = db.CommentRatings.Find(id);
            db.CommentRatings.Remove(commentRating);
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
