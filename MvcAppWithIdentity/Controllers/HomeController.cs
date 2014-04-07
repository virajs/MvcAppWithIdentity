using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace MvcAppWithIdentity.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(string username, string password, string firstname, 
            string lastname, string birthdate, int age)
        {
            var mgr = Identity.GetUserManager();
            var user = new AppUser
            {
                UserName = username,
                FirstName = firstname,
                LastName = lastname,
                BirthDate = DateTime.Parse(birthdate), // shoud use TryParse here :)
                Age = age
            };

            var result = await mgr.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return View("CreateSuccess");
            }
            foreach (var msg in result.Errors)
            {
                ModelState.AddModelError("", msg);
            }
            return View();
        }
        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            var mgr = Identity.GetUserManager();
            var user = await mgr.FindAsync(username, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username/password");
                return View();
            }

            //if (mgr.GetTwoFactorEnabled(user.Id))
            //{
            //    var rememberMe = await Request.GetOwinContext().Authentication.AuthenticateAsync("RememberBrowserTwoFactor");
            //    if (rememberMe == null || rememberMe.Identity.GetUserId() != user.Id)
            //    {
            //        var code = mgr.GenerateTwoFactorToken(user.Id, "mobile");
            //        mgr.NotifyTwoFactorToken(user.Id, "mobile", code);

            //        var id = new ClaimsIdentity("TwoFactor");
            //        id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            //        Request.GetOwinContext().Authentication.SignIn(id);
            //        return RedirectToAction("TwoFactor");
            //    }
            //}

            {
                var id = await mgr.CreateIdentityAsync(user, "Cookies");
                id.AddClaim(new Claim("fullname", string.Format("{0} {1}", 
                    user.FirstName.ToString(), user.LastName.ToString())));

                Request.GetOwinContext().Authentication.SignIn(id);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index");
        }
        
        public async Task<ActionResult> Update()
        {
            var id = User.Identity.GetUserId();
            var mgr = Identity.GetUserManager();
            
            var user = await mgr.FindByIdAsync(id);
            user.Age++;
            mgr.Update(user);

            mgr.AddClaim(id, new Claim(ClaimTypes.Role, "Appointment"));
            //mgr.AddClaim(id, new Claim("age", 12));
            //var roleMgr = Identity.GetRoleManager();
            //if (!await roleMgr.RoleExistsAsync("Customer"))
            //{
                //roleMgr.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Customer"));
            //}
            //mgr.AddToRole(id, "Customer");


            //var result = mgr.SetEmail(id, "brockallen@gmail.com");
            //var token = mgr.GenerateEmailConfirmationToken(id);
            //mgr.SendEmail(id, ....)
            //mgr.ConfirmEmail(id, token);

            //mgr.ChangePassword(id, "")
            //var id = mgr.FindByEmail()
            //var resetToken = mgr.GeneratePasswordResetToken(id);
            //mgr.SendEmail();
            //mgr.ResetPassword(id, resetToken, "pass123");

            
            //mgr.SetPhoneNumber(id, "123");
            //mgr.ChangePhoneNumber()
            //var changePhoneToken = mgr.GenerateChangePhoneNumberToken(id, "123");
            //mgr.SendSms(id, "Code: " + changePhoneToken);
            //mgr.ChangePhoneNumber(id, "123", changePhoneToken);

            //mgr.SetTwoFactorEnabled(id, true);

            //mgr.AddLogin(id, new UserLoginInfo("Google", "123"));
            //mgr.Find()

            return RedirectToAction("Index");
        }

        public ActionResult TwoFactor()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> TwoFactor(string code)
        {
            var ctx = Request.GetOwinContext();
            var result = await ctx.Authentication.AuthenticateAsync("TwoFactor");
            if (result != null)
            {
                var id = result.Identity.GetUserId();
                var mgr = Identity.GetUserManager();
                if (mgr.VerifyTwoFactorToken(id, "mobile", code))
                {
                    ctx.Authentication.SignOut("TwoFactor");
                    var user = mgr.FindById(id);
                    var ci = mgr.CreateIdentity(user, "Cookies");
                    //ctx.Authentication.SignIn(ci);

                    var remember = new ClaimsIdentity("RememberBrowserTwoFactor");
                    remember.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                    ctx.Authentication.SignIn(ci, remember);

                    return RedirectToAction("Index", "Home");
                }
            }
            
            return RedirectToAction("Index");
        }
    }
}