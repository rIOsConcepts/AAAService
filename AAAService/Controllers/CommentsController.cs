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
    public class CommentsController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: Comments
        public ActionResult Index()
        {
            var errors_and_comments = db.errors_and_comments.Include(e => e.CommentRating);
            return View(errors_and_comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            errors_and_comments errors_and_comments = db.errors_and_comments.Find(id);
            if (errors_and_comments == null)
            {
                return HttpNotFound();
            }
            return View(errors_and_comments);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.RatingID = new SelectList(db.CommentRatings, "RatingID", "Name");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "guid,ID,HTTP_REFERER,HTTP_USER_AGENT,REMOTE_ADDR,REMOTE_HOST,REMOTE_USER,comment_datetime,comment_type,userguid,comments,fileupload,active,closed,notes,ratings,RatingID")] errors_and_comments errors_and_comments)
        {
            if (ModelState.IsValid)
            {
                errors_and_comments.guid = Guid.NewGuid();
                db.errors_and_comments.Add(errors_and_comments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RatingID = new SelectList(db.CommentRatings, "RatingID", "Name", errors_and_comments.RatingID);
            return View(errors_and_comments);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            errors_and_comments errors_and_comments = db.errors_and_comments.Find(id);
            if (errors_and_comments == null)
            {
                return HttpNotFound();
            }
            ViewBag.RatingID = new SelectList(db.CommentRatings, "RatingID", "Name", errors_and_comments.RatingID);
            return View(errors_and_comments);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "guid,ID,HTTP_REFERER,HTTP_USER_AGENT,REMOTE_ADDR,REMOTE_HOST,REMOTE_USER,comment_datetime,comment_type,userguid,comments,fileupload,active,closed,notes,ratings,RatingID")] errors_and_comments errors_and_comments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(errors_and_comments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RatingID = new SelectList(db.CommentRatings, "RatingID", "Name", errors_and_comments.RatingID);
            return View(errors_and_comments);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            errors_and_comments errors_and_comments = db.errors_and_comments.Find(id);
            if (errors_and_comments == null)
            {
                return HttpNotFound();
            }
            return View(errors_and_comments);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            errors_and_comments errors_and_comments = db.errors_and_comments.Find(id);
            db.errors_and_comments.Remove(errors_and_comments);
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
