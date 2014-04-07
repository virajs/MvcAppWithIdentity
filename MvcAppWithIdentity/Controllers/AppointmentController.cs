using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAppWithIdentity.Controllers
{
    [Authorize(Roles="Appointment")]
    public class AppointmentController : Controller
    {
        //
        // GET: /Appointment/
        public ActionResult Index()
        {
            return View();
        }
	}
}