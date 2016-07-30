using AAAService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AAAService.Helpers
{
    class SvcHelper
    {
        public static Guid getLocation()
        {
            aaahelpEntities db = new aaahelpEntities();
            ApplicationDbContext dbI = new ApplicationDbContext();
            var user = dbI.Users.FirstOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
            var myuserguid = user.guid;
            var mylist = from c in db.user_to_location
                         where c.user_guid.Equals(myuserguid)
                         select c;

            var x = mylist != null ? mylist.Count() : 0;

            if (x > 0)
            {
                var mylocationnguid = mylist.First().location_guid;

                return mylist.First().location_guid;
            }
            else
            {
                return Guid.Parse("6FFB64D7-4D69-4F1C-BC55-5376588A39F4");
            }
        }

        public static int getnumLocations()
        {
            aaahelpEntities db = new aaahelpEntities();
            ApplicationDbContext dbI = new ApplicationDbContext();
            var user = dbI.Users.FirstOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
            var myuserguid = user.guid;
            var mylist = from c in db.user_viewable_locations
                         where c.user_guid.Equals(myuserguid)
                         select c;
            var x = mylist.Count();
            
            return x;
        }
        public static int getSvcIDMax()
        {
            aaahelpEntities db = new aaahelpEntities();
            var item = db.service_tickets.OrderByDescending(i => i.id).FirstOrDefault();
            int maxnum = item.id;
            return maxnum;
        }

        public static int getjobNumMax()
        {
            aaahelpEntities db = new aaahelpEntities();
            var item = db.service_tickets.OrderByDescending(i => i.job_number).FirstOrDefault();
            int maxnum = item.job_number;
            return maxnum;
        }
        public static int getBidMax()
        {
            aaahelpEntities db = new aaahelpEntities();
            var item = db.bid_requests.OrderByDescending(i => i.bid_num).FirstOrDefault();
            int maxnum = item.bid_num;
            return maxnum;
        }

        public static string getCreatedby(Guid username)
        {
            ApplicationDbContext dbI = new ApplicationDbContext();
            var found = dbI.Users.FirstOrDefault(u => u.guid.Equals(username));
            var myusername = found.fname + " " + found.lname;
            return myusername;
        }
        /* public static Guid getParentGuid(IQueryable<System.Guid> mylocguid)

         {
             aaahelpEntities dbI = new aaahelpEntities();
             // create Tuple to store location info for posting to bid or service request
             // convert mylocguid back to Guid to compare using Equals for Guid
             var found = dbI.locationinfoes.FirstOrDefault(u => u.guid.Equals(mylocguid));
             return Cast<found.parentguid>;*/






    }

}
