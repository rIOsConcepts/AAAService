using AAAService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AAAService.Controllers
{
    [Authorize(Roles = "SiteAdmin")]
    public class UsersAdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private aaahelpEntities db = new aaahelpEntities();

        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {            
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        //Action result for ajax call 
        [HttpPost]
        public ActionResult GetUserStatuses()
        {
            SelectList obj = new SelectList(db.OtherStatusLists.Where(o => o.Active == true).OrderBy(o => o.Name), "Value", "Name");
            return Json(obj);
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {                                    
            return View(await UserManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            //ViewBag.PhoneNumbers = db.phone_num.Where(pn => pn.user_guid == user.guid);
            var userToLocation = db.user_to_location.Where(utl => utl.user_guid == user.guid).Include(l => l.locationinfo).ToList();
            var locationsOfUser = userToLocation.Select(o => o.location_guid.ToString()).ToList();
            var locations = db.locationinfoes.Where(l => locationsOfUser.Contains(l.guid.ToString())).OrderBy(l => l.name).ToList();
            
            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            return View(new Tuple<ApplicationUser, IEnumerable<locationinfo>>(user, locations));
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var userguid = Guid.NewGuid();
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email, fname = userViewModel.fname, lname = userViewModel.lname, title= userViewModel.title, guid = userguid };
                
                // Create user active by default (per request on 6/23/2016)
                user.account_status = 1;
                var password = String.Empty;

                do
                {
                    password = Membership.GeneratePassword(6, 1);
                }
                while (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,}"));

                var result = await UserManager.CreateAsync(user, password);

                //if (result.Succeeded)
                //{
                //    //DO NOT AUTOLOGIN CREATED USER
                //    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //    // Send an email with this link
                //    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                //    return RedirectToAction("Index", "Home");
                //}

                //AddErrors(result);

                try
                {
                    var email = new Correspondence.Mail();
                    var callbackUrl = "http://assetsaaa.com/Register?guid=" + user.Id;

                    var body = "Welcome. You have been invited to use the AAA Property Services customer portal. This system allows you to report problems with your facilities so that they can be resolved as quickly as possible. To setup your user, please follow the link below:<br><br>" +
                               "<a href=\"" + callbackUrl + "\">Click here</a><br><br>" +
                               "Or cut and paste the following into your web browser.<br><br>" +
                               callbackUrl + "<br><br>" +
                               "Your temporary password is: " + password + "<br><br>" +
                               "Password requirements:<br>" +
                               "<ul><li>The Password must be at least 6 characters long.</li>" + 
                               "<li>Passwords must have at least one non letter or digit character.</li>" + 
                               "<li>Passwords must have at least one lowercase ('a'-'z').</li>" + 
                               "<li>Passwords must have at least one uppercase ('A'-'Z').</li></ul>" +
                               "Thank You<br><br>" +
                               "If you have questions or concerns about this message please contact us at 1-800-892-4784.<br>" +
                               "Please do not reply to this e-mail, this account is not monitored.";

                    email.Send(subject:"Web Portal User Invitation", body:body, isHTML:true, email:user.Email);
                }
                catch (Exception e)
                {
                    System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
                }

                //Add User to the selected Roles 
                if (result.Succeeded)
                {
                    //MR:Saing User's Phone Numbers
                    if (userViewModel.BusinessPhoneNumber != null || userViewModel.AfterHoursPhoneNumber != null)
                    {
                        var phone_num = new phone_num();
                        phone_num.guid = Guid.NewGuid();
                        phone_num.user_guid = user.guid;
                        phone_num.phone_day = userViewModel.BusinessPhoneNumber;
                        phone_num.phone_night = userViewModel.AfterHoursPhoneNumber;
                        db.phone_num.Add(phone_num);                        
                    }

                    //MR:Saving User's Locations
                    var locations = Request.Form["to[]"] != null ? Request.Form["to[]"].Split(',') : new string[0];

                    if (locations.Count() > 0)
                    {
                        foreach (var location in locations)
                        {
                            var user_to_location = new user_to_location();
                            user_to_location.guid = Guid.NewGuid();
                            user_to_location.user_guid = userguid;
                            user_to_location.location_guid = Guid.Parse(location);
                            db.user_to_location.Add(user_to_location);                            
                        }
                    }

                    db.SaveChanges();

                    if (selectedRoles != null)
                    {
                        var resultRoles = await UserManager.AddToRolesAsync(user.Id, selectedRoles);

                        if (!resultRoles.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();
                }

                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var phoneNumbers = db.phone_num.Where(o => o.user_guid == user.guid);
            var phoneNumbersFound = new phone_num();

            if (phoneNumbers.Count() > 0)
            {
                phoneNumbersFound = phoneNumbers.ToList()[0];
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            ViewBag.StatusID = new SelectList(db.OtherStatusLists.Where(o => o.Active == true), "Value", "Name", user.account_status);
            ViewBag.Companies = db.Companies.Where(o => o.active == true).OrderBy(o => o.name);
            ViewBag.Locations = db.locationinfoes.Where(o => o.active == true).OrderBy(o => o.name);

            //var userCompanies = db.user_to_location
            //                    .Where(utl => utl.user_guid == user.guid)
            //                    .Join(db.locationinfoes, utl => utl.location_guid, l => l.guid, (utl, l) => new { utl, l })
            //                    .Join(db.Companies, lpc => lpc.l.parentguid, c => c.guid, (lpc, c) => new { lpc, c })

            //                    .Select(m => new StronglyTyped()
            //                    {
            //                        GUID = m.c.guid,
            //                        Name = m.c.name
            //                    })
            //                    .OrderBy(st => st.Name);

            var userCompanies = db.user_to_location
                                .Where(utl => utl.user_guid == user.guid)
                                .Join(db.locationinfoes, utl => utl.location_guid, l => l.guid, (utl, l) => new { utl, l })
                                .Join(db.Companies, lpc => lpc.l.parentguid, c => c.guid, (lpc, c) => new { lpc, c })
                                .GroupBy(g => new StronglyTyped()
                                {
                                    GUID = g.c.guid,
                                    Name = g.c.name
                                })
                                .Select(m => new StronglyTyped()
                                {
                                    GUID = m.Key.GUID,
                                    Name = m.Key.Name
                                })                                
                                .OrderBy(st => st.Name);

            ViewBag.UserCompanies = userCompanies.ToList();

            var userLocations = db.user_to_location
                                .Where(utl => utl.user_guid == user.guid)
                                .Join(db.locationinfoes, utl => utl.location_guid, l => l.guid, (utl, l) => new { utl, l })

                                .Select(m => new StronglyTyped()
                                {
                                    GUID = m.l.guid,
                                    Name = m.l.name,
                                    ParentGUID = m.l.parentguid
                                }).OrderBy(st => st.Name);

            ViewBag.UserLocations = userLocations.ToList();

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                fname = user.fname,
                lname = user.lname,
                title = user.title,
                Email = user.Email,
                BusinessPhoneNumber = phoneNumbersFound.phone_day,
                AfterHoursPhoneNumber = phoneNumbersFound.phone_night,

                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,lname,fname,title,BusinessPhoneNumber,AfterHoursPhoneNumber")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.fname = editUser.fname;
                user.lname = editUser.lname;
                user.title = editUser.title;
                var statusID = int.Parse(Request.Form["StatusID"]);
                user.account_status = statusID;

                //MR:Saing User's Phone Numbers
                if (editUser.BusinessPhoneNumber != null || editUser.AfterHoursPhoneNumber != null)
                {
                    var phoneNum = db.phone_num.Where(o => o.user_guid == user.guid);

                    if (phoneNum.Count() > 0)
                    {
                        phoneNum.ToList()[0].phone_day = editUser.BusinessPhoneNumber;
                        phoneNum.ToList()[0].phone_night = editUser.AfterHoursPhoneNumber;
                    }
                    else
                    {
                        var phone_num = new phone_num();
                        phone_num.guid = Guid.NewGuid();
                        phone_num.user_guid = user.guid;
                        phone_num.phone_day = editUser.BusinessPhoneNumber;
                        phone_num.phone_night = editUser.AfterHoursPhoneNumber;
                        db.phone_num.Add(phone_num);
                    }
                }
                else
                {
                    var phoneNumbersToDelete = from pn in db.phone_num
                                               where pn.user_guid == user.guid
                                               select pn;

                    if (phoneNumbersToDelete.Count() > 0)
                    {
                        db.phone_num.RemoveRange(phoneNumbersToDelete);
                    }
                }

                //MR:Saving User's Locations
                var locations = Request.Form["to[]"] != null ? Request.Form["to[]"].Split(',') : new string[0];

                if (locations.Count() > 0)
                {
                    var newLocations = new List<Guid>();
                    var userLocations = db.user_to_location.Where(utl => utl.user_guid == user.guid);
                    var userLocationsAdded = new List<Guid>();

                    foreach (var location in locations)
                    {
                        newLocations.Add(Guid.Parse(location));
                        var GUIDLocation = Guid.Parse(location);

                        if (userLocations.Where(utl => utl.location_guid == GUIDLocation).Count() == 0)
                        {
                            userLocationsAdded.Add(GUIDLocation);
                            var user_to_location = new user_to_location();
                            user_to_location.guid = Guid.NewGuid();
                            user_to_location.user_guid = user.guid;
                            user_to_location.location_guid = GUIDLocation;
                            db.user_to_location.Add(user_to_location);
                        }                        
                    }

                    db.SaveChanges();
                    var allUserLocationsInDB = db.user_to_location.Where(utl => utl.user_guid == user.guid).ToList();

                    //MR:Deleting Locations where the user is not assign to anymore
                    var userLocationsToDelete = from ul in allUserLocationsInDB
                                                where !newLocations.Contains(ul.location_guid)
                                                select ul;

                    if (userLocationsToDelete.Count() > 0)
                    {
                        db.user_to_location.RemoveRange(userLocationsToDelete);
                        db.SaveChanges();
                    }                    
                }
                else
                {
                    db.user_to_location.RemoveRange(db.user_to_location.Where(utl => utl.user_guid == user.guid));
                    db.SaveChanges();
                }

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByEmailAsync(email);

            //if (user == null)
            //{
            //    return HttpNotFound();
            //}

            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed([Bind(Include = "guid,fname,lname,title,account_status,is_manager")] ApplicationUser deleteUser)
        public async Task<ActionResult> DeleteConfirmed(string email)
        {
            if (ModelState.IsValid)
            {
                if (email == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByEmailAsync(email);

                //if (user == null)
                //{
                //    return HttpNotFound();
                //}

                IdentityResult result = new IdentityResult();

                try
                {
                    var phoneNumbersToDelete = from pn in db.phone_num
                                               where pn.user_guid == user.guid
                                               select pn;

                    if (phoneNumbersToDelete.Count() > 0)
                    {
                        db.phone_num.RemoveRange(phoneNumbersToDelete);
                        db.SaveChanges();
                    }

                    var userLocationToDelete = from utl in db.user_to_location
                                               where utl.user_guid == user.guid
                                               select utl;

                    if (userLocationToDelete.Count() > 0)
                    {
                        db.user_to_location.RemoveRange(userLocationToDelete);
                        db.SaveChanges();
                    }

                    result = await UserManager.DeleteAsync(user);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message + "\r\n\\" + e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : "" : "");
                }                
            }

            return View();
        }
    }
}