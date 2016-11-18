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
            //var errors_and_comments = db.errors_and_comments.Include(e => e.CommentRating);
            var errors_and_comments = db.error_and_comments_view.OrderByDescending(eac => eac.ID);
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
                var user = Helpers.UserHelper.getUserGuid();
                errors_and_comments.guid = Guid.NewGuid();
                errors_and_comments.ID = db.errors_and_comments.Max(x => x.ID) + 1;
                errors_and_comments.comment_datetime = DateTime.Now;
                errors_and_comments.comment_type = "Comment";
                errors_and_comments.userguid = user;
                errors_and_comments.active = true;
                errors_and_comments.ratings = db.CommentRatings.Where(o => o.RatingID == errors_and_comments.RatingID).ToList()[0].RatingValue;
                db.errors_and_comments.Add(errors_and_comments);
                db.SaveChanges();

                try
                {
                    var email = new Correspondence.Mail();
                    var userData = db.AspNetUsers.Where(o => o.guid == user).ToList()[0];
                    var rating = db.CommentRatings.Where(cr => cr.RatingID == errors_and_comments.RatingID);
                    var phoneNum = db.phone_num.Where(pn => pn.user_guid == userData.guid);
                    var userToLocationData = db.user_to_location.Where(utl => utl.user_guid == userData.guid);
                    locationinfo location = null;

                    if (userToLocationData.Count() > 0)
                    {
                        var userToLocation = userToLocationData.ToList()[0];
                        var locationData = db.locationinfoes.Where(l => l.guid == userToLocation.location_guid);

                        if (locationData.Count() > 0)
                        {
                            location = locationData.ToList()[0];
                        }
                    }

                    var body = "Comment Or Error From AAA Web Portal" + "\r\n" +
                               "Comment Type: " + errors_and_comments.comment_type + "\r\n" +
                               "Comment Rating: " + (rating.Count() > 0 ? rating.ToList()[0].Name : "") + "\r\n" +
                               "Comments: " + errors_and_comments.comments + "\r\n" +
                               "Comment date and time: " + errors_and_comments.comment_datetime + "\r\n" +
                               "Comment Made By: " + userData.fname + " " + userData.lname + "\r\n" +
                               "Users Email: " + userData.Email + "\r\n" +
                               "Day Phone: " + (phoneNum.Count() > 0 ? phoneNum.ToList()[0].phone_day : "") + "\r\n" +
                               "Night Phone: " + (phoneNum.Count() > 0 ? phoneNum.ToList()[0].phone_night : "") + "\r\n" +
                               "Users Site: " + (location != null ? location.name : "") + "\r\n" +
                               "Site Address: " + (location != null ? location.addressline1 : "") + "\r\n" +
                               "Site Address: " + (location != null ? location.city + ", " + location.state + " " + location.zip : "") + "\r\n" +
                               "User Came From Page: http://assetsaaa.com/Comments/Create" + "\r\n" +
                               "This Is Comment Number: " + errors_and_comments.ID;

                    email.Send(subject: "AAAWebPortalCommentsErrors", body: body, email: userData.Email);
                }
                catch (Exception e)
                {
                    System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                }

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
            TempData["CommentsID"] = errors_and_comments.ID;
            return View(errors_and_comments);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "guid,ID,HTTP_REFERER,HTTP_USER_AGENT,REMOTE_ADDR,REMOTE_HOST,REMOTE_USER,comment_datetime,comment_type,userguid,comments,fileupload,active,closed,notes,ratings,RatingID")] errors_and_comments errors_and_comments)
        //Manuel Rios: You should only include properties in the bind attribute that you want to change
        public ActionResult Edit([Bind(Include = "guid,comment_datetime,comment_type,userguid,comments,fileupload,active,closed,notes,ratings,RatingID")] errors_and_comments errors_and_comments)
        {
            var iD = TempData["CommentsID"].ToString();            
            errors_and_comments.ID = int.Parse(iD);

            if (ModelState.IsValid)
            {
                try
                {                    
                    //Per Tim's request on 11/16/2016, not to overwrite original date the comment was created
                    //errors_and_comments.comment_datetime = DateTime.Now;
                    errors_and_comments.comment_type = "Comment";
                    //Per Tim's request on 11/16/2016, not to overwrite original user that created the comment
                    //errors_and_comments.userguid = Helpers.UserHelper.getUserGuid();
                    db.Entry(errors_and_comments).State = EntityState.Modified;
                    db.SaveChanges();

                    try
                    {
                        var email = new Correspondence.Mail();
                        var userData = db.AspNetUsers.Where(o => o.guid == errors_and_comments.userguid).ToList()[0];
                        var rating = db.CommentRatings.Where(cr => cr.RatingID == errors_and_comments.RatingID);
                        var phoneNum = db.phone_num.Where(pn => pn.user_guid == userData.guid);
                        var userToLocationData = db.user_to_location.Where(utl => utl.user_guid == userData.guid);
                        locationinfo location = null;

                        if (userToLocationData.Count() > 0)
                        {
                            var userToLocation = userToLocationData.ToList()[0];
                            var locationData = db.locationinfoes.Where(l => l.guid == userToLocation.location_guid);

                            if (locationData.Count() > 0)
                            {
                                location = locationData.ToList()[0];
                            }
                        }

                        var body = "Comment Or Error From AAA Web Portal" + "\r\n" +
                                   "Comment Type: " + errors_and_comments.comment_type + "\r\n" +
                                   "Comment Rating: " + (rating.Count() > 0 ? rating.ToList()[0].Name : "") + "\r\n" +
                                   "Comments: " + errors_and_comments.comments + "\r\n" +
                                   "Comment date and time: " + errors_and_comments.comment_datetime + "\r\n" +
                                   "Comment Made By: " + userData.fname + " " + userData.lname + "\r\n" +
                                   "Notes: " + errors_and_comments.notes + "\r\n" +
                                   "Users Email: " + userData.Email + "\r\n" +
                                   "Day Phone: " + (phoneNum.Count() > 0 ? phoneNum.ToList()[0].phone_day : "") + "\r\n" +
                                   "Night Phone: " + (phoneNum.Count() > 0 ? phoneNum.ToList()[0].phone_night : "") + "\r\n" +
                                   "Users Site: " + (location != null ? location.name : "") + "\r\n" +
                                   "Site Address: " + (location != null ? location.addressline1 : "") + "\r\n" +
                                   "Site Address: " + (location != null ? location.city + ", " + location.state + " " + location.zip : "") + "\r\n" +
                                   "User Came From Page: http://assetsaaa.com/Comments/Create" + "\r\n" +
                                   "This Is Comment Number: " + errors_and_comments.ID;

                        email.Send(subject: "AAAWebPortalCommentsErrors", body: body, email: userData.Email);
                    }
                    catch (Exception e)
                    {
                        System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                    }

                    return RedirectToAction("Index");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }

                    throw;
                }
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