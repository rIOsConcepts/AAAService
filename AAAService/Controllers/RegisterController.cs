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
    public class RegisterController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        public RegisterController()
        {
        }

        public RegisterController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
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
        // GET:
        public async Task<ActionResult> Index()
        {
            var id = Request.QueryString["guid"];

            if (id != "")
            {
                var user = await UserManager.FindByIdAsync(id);
                //var userguid = Guid.Parse(guidParameter);
                //var user = UserManager.Users.Where(o => o.guid == userguid).ToList()[0];

                return View(new PasswordViewModel()
                {
                    Id = user.Id,
                    FName = user.fname,
                    LName = user.lname,
                    Title = user.title,
                    Email = user.Email,
                    Password = null,
                    ConfirmPassword = null
                });
            }
            else
            {
                return View();
            }
        }

        //
        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind(Include = "Id, lname, fname, title, Email, TemporaryPassword, Password, ConfirmPassword")] PasswordViewModel editUser)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                var passwordHasher = new PasswordHasher();
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user.PasswordHash, editUser.TemporaryPassword);

                if (passwordVerificationResult.ToString() != "Succeeded")
                {
                    ModelState.AddModelError("", "Temporary password doesn't match, please verify in email invitation received.");
                    return View();
                }

                user.UserName = editUser.Email;
                user.fname = editUser.FName;
                user.lname = editUser.LName;
                user.title = editUser.Title;

                var adminresult = await UserManager.UpdateAsync(user);

                if (adminresult.Succeeded)
                {
                    adminresult = UserManager.RemovePassword(editUser.Id);

                    if (adminresult.Succeeded)
                    {
                        adminresult = UserManager.AddPassword(editUser.Id, editUser.Password);

                        if (adminresult.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            foreach (var error in adminresult.Errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                        }
                    }
                }                
            }

            return View();
        }
    }
}