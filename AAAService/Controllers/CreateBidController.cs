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
using System.Web.Routing;

namespace AAAService.Controllers
{
    [Authorize]
    public class CreateBidController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: CreateTicket
        public ActionResult Index()
        {
            var myuserguid = AAAService.Helpers.UserHelper.getUserGuid();
            var mylocationguid = AAAService.Helpers.SvcHelper.getLocation();

            if (mylocationguid == Guid.Parse("6FFB64D7-4D69-4F1C-BC55-5376588A39F4"))
            {
                return View("NoLocation");
            }
            else
            {
                // Check to see if that have more than one viewable location
                var numlocs = AAAService.Helpers.SvcHelper.getnumLocations();

                if (numlocs > 1)
                {
                    // They do have at least two, so let them choose which one
                    var multilocs = from s in db.user_viewable_locations_view
                                    select s;
                    multilocs = multilocs.Where(s => s.user_guid == myuserguid).OrderByDescending(s => s.name);

                    return View(multilocs.ToList<user_viewable_locations_view>());
                }
                else
                {
                    // No viewable locations so just use the standard location from user_to_location table



                    return RedirectToAction("Create", new RouteValueDictionary(
                           new { controller = "BidRequests", action = "Create", Id = mylocationguid }));

                }
            }


        }

        // Oooops!: they got here because they do not have an assigned location.
        public ActionResult NoLocation()
        {

            return View();
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

