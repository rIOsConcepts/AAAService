using AAAService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AAAService.Helpers
{
    class UserHelper
    {
        public static object svcuserguid { get; private set; }

        public static string getUserId()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
            var usernem = db.Users.Where(p => p.guid.Equals(svcuserguid));
            return user.Id;
        }

        public static Guid getUserGuid()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
            return user.guid;
        }


        public static string getUserPhones()


        {
            ApplicationDbContext db = new ApplicationDbContext();
            aaahelpEntities dbI = new aaahelpEntities();

            var user = db.Users.FirstOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
            var myguid = user.guid ;
            var phones = dbI.phone_num.FirstOrDefault(t => t.user_guid == myguid);
            var allPhones = phones.phone_day + "#" + phones.phone_night;
            return allPhones;
        }
        public static string getUserName()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
            var myfullname = user.fname + ' ' + user.lname;
            return myfullname;
        }


    }
   
}
