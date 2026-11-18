using DRLRelease.Notification;
using DRLRelease.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DRLRelease.Web.Controllers
{
    [AuthorizeWebForm]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Test(TestNotificationModel model, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(model.DeviceId) && !string.IsNullOrWhiteSpace(model.Message))
            {
                if (Pusher.SendToiOSDevice(model.DeviceId, model.Message))
                {

                }
            }
            return View(model);
        }
    }
}
