using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Owin;
using Microsoft.Owin.Security.DataProtection;

namespace MvcAppWithIdentity
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public int Age { get; set; }
    }
    
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager()
            : base(new UserStore<AppUser>(new AppDatabase()))
        {
        }
    }

    public class AppDatabase : IdentityDbContext<AppUser>
    {
    }

    public class Identity
    {
        static IDataProtector dataProtector;
        public static void Configure(IAppBuilder app)
        {
            dataProtector = app.GetDataProtectionProvider().Create("EmailVerification");
        }

        public static AppUserManager GetUserManager()
        {
            var mgr = new AppUserManager();
            mgr.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
            };
            mgr.EmailService = new Email();
            mgr.SmsService = new Email();
            mgr.ClaimsIdentityFactory = new Factory(mgr.ClaimsIdentityFactory);
            mgr.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(dataProtector);
            mgr.TwoFactorProviders.Add("mobile", new PhoneNumberTokenProvider<AppUser, string>());
            return mgr;
        }

        public static RoleManager<IdentityRole> GetRoleManager()
        {
            var mgr = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new AppDatabase()));
            return mgr;
        }
    }

    public class Email : IIdentityMessageService
    {
        public System.Threading.Tasks.Task SendAsync(IdentityMessage message)
        {
            return Task.FromResult(0);
        }
    }

    public class Factory : IClaimsIdentityFactory<AppUser, string>
    {
        IClaimsIdentityFactory<AppUser, string> inner;
        public Factory(IClaimsIdentityFactory<AppUser, string> inner)
        {
            this.inner = inner;
        }
        public async Task<ClaimsIdentity> CreateAsync(UserManager<AppUser, string> manager, AppUser user, string authenticationType)
        {
            var id = await inner.CreateAsync(manager, user, authenticationType);
            id.AddClaim(new Claim("age", user.Age.ToString()));
            return id;
        }
    }

}