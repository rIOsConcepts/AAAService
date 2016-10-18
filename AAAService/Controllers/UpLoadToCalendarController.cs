using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AAAService.Models;

namespace AAAService.Controllers
{
    public class UpLoadToCalendarController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        [Authorize(Roles = "SiteAdmin")]
        // GET: UpLoadToCalendar
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    file.SaveAs(path);

                    var calendars = db.Calendars.Find(4);
                    calendars.Id = 4;
                    calendars.Path = "~/" + path.Substring(path.IndexOf("Images", 0), path.Length - path.IndexOf("Images", 0)).Replace("\\", "/");
                    db.SaveChanges();
                }

                ViewBag.Message = "Upload successful";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
    }
}