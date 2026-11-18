using DRLRelease.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DRLRelease.Web.Controllers
{
    public class AuthorizeWebFormAttribute : AuthorizeAttribute
    {
        //
        // GET: /AuthorizeWebFormAttribute/
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var loginUrl = "~/Account/LogIn";
            if (HttpContext.Current.Request.Cookies["UserID"] == null || SessionManager.Instance.UserName == null || HttpContext.Current.Session.Count == 0)
            {
                filterContext.Result = new RedirectResult(loginUrl);
            }
            //if (Convert.ToString(filterContext.RouteData.Values["controller"]) == "POS")
            //{
            //    if (Convert.ToInt32(SessionManager.Instance.POSShiftLogged) < 1)
            //    {
            //        filterContext.Result = new RedirectResult("~/");
            //    }
            //}
            //else
            //{
            //    if (ActiveMachines.lstActiveMachines.Any(x => x.MachineName == SessionManager.Instance.MachineName && x.LastUpdate.AddMinutes(1) < DateTime.Now))
            //        filterContext.Result = new RedirectResult(loginUrl);
            //}
        }

    }
}
