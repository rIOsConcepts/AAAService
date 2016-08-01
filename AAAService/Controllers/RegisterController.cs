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
        private ApplicationSignInManager _signInManager;
        private aaahelpEntities db = new aaahelpEntities();

        public RegisterController()
        {
        }

        public RegisterController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;            
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind(Include = "Id, lname, fname, title, Email, TemporaryPassword, Password, ConfirmPassword")] PasswordViewModel editUser)
        {
            if (!ModelState.IsValid)
            {
                return View(editUser);
            }

            //var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), editUser.TemporaryPassword, editUser.Password);
            var result = await UserManager.ChangePasswordAsync(editUser.Id, editUser.TemporaryPassword, editUser.Password);

            if (result.Succeeded)
            {
                //var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var user = await UserManager.FindByIdAsync(editUser.Id);
                
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return RedirectToAction("Index", "Home", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            AddErrors(result);
            return View(editUser);

            //if (result.ToString() != "Succeeded")
            //{
            //    ModelState.AddModelError("", "Temporary password doesn't match, please verify in email invitation received.");
            //    return View();
            //}

            //user.UserName = editUser.Email;
            //user.fname = editUser.FName;
            //user.lname = editUser.LName;
            //user.title = editUser.Title;

            //var adminresult = await UserManager.UpdateAsync(user);

            //if (adminresult.Succeeded)
            //{
            //    adminresult = UserManager.RemovePassword(editUser.Id);

            //    if (adminresult.Succeeded)
            //    {
            //        adminresult = UserManager.AddPassword(editUser.Id, editUser.Password);

            //        if (adminresult.Succeeded)
            //        {
            //            return RedirectToAction("Index", "Home");
            //        }
            //        else
            //        {
            //            foreach (var error in adminresult.Errors)
            //            {
            //                ModelState.AddModelError("", error);
            //            }
            //        }
            //    }
            //}                
            //}

            //return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
    }
}