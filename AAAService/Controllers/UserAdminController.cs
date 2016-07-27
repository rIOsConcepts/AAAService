using AAAService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AAAService.Controllers
{
    [Authorize(Roles = "SiteAdmin")]
    public class UsersAdminController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
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

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
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
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email,fname = userViewModel.fname,lname = userViewModel.lname,title= userViewModel.title,guid = userguid };
                
                // Create user active by default (per request on 6/23/2016)
                user.account_status = 1;
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
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

                        db.SaveChanges();
                    }

                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
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
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,lname,fname,title")] EditUserViewModel editUser, params string[] selectedRole)
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
        public async Task<ActionResult> Delete(string id)
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
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
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
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}