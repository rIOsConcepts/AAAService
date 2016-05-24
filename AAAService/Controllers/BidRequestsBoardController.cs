using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AAAService.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using AAAService.Helpers;

namespace AAAService.Controllers
{
    [Authorize]
    public class BidRequestsBoardController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();
        ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
        // GET: BidRequestsBoard
        public ActionResult Index()
        {
            var myuserguid = user.guid;

            var mylist = from c in db.user_to_location
                         where c.user_guid.Equals(myuserguid)
                         select c;
            var x = mylist.Count();
            var mylocationnguid = mylist.First().location_guid;


            var mylist2 = from d in db.locationinfoes
                          where d.guid.Equals(mylocationnguid)
                          select d;

            var mycompanyguid = mylist2.First().parentguid;

            if (mylocationnguid.Equals(mycompanyguid))
            {
                x = 2;
            }

            if (mylist == null)
            {
                return View("Details");
            }
            else
            {



                if (x > 1)
                {
                    var multilocs = from s in db.Bid_Requests_View
                                    select s;
                    multilocs = multilocs.Where(s => s.parent_company_guid == mycompanyguid).OrderByDescending(s => s.bid_num);

                    return View(multilocs.ToList<Bid_Requests_View>());

                }
                else
                {
                    var singleloc = from s in db.Bid_Requests_View
                                    select s;
                    singleloc = singleloc.Where(s => s.service_location_guid == (mylocationnguid)).OrderByDescending(s => s.bid_num);

                    return View(singleloc.ToList<Bid_Requests_View>());
                }



            }

        }
    

        // GET: BidRequestsBoard/Details/5
        public ActionResult Details()
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
