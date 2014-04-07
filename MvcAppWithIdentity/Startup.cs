using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppWithIdentity
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Identity.Configure(app);

            var opts = new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                ExpireTimeSpan = TimeSpan.FromMinutes(20)
            };
            app.UseCookieAuthentication(opts);

            //var opts2 = new CookieAuthenticationOptions
            //{
            //    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
            //    AuthenticationType = "TwoFactor",
            //    ExpireTimeSpan = TimeSpan.FromMinutes(5)
            //};
            //app.UseCookieAuthentication(opts2);

            //var opts3 = new CookieAuthenticationOptions
            //{
            //    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
            //    AuthenticationType = "RememberBrowserTwoFactor",
            //    ExpireTimeSpan = TimeSpan.FromDays(30)
            //};
            //app.UseCookieAuthentication(opts3);
        }
    }
}