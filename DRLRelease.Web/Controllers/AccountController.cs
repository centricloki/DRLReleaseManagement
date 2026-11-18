using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DRLRelease.Web.Filters;
using DRLRelease.Web.Models;
using DRLRelease.Business;
using DRLRelease.Common;
using DRLRelease.DataModel;
//using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

namespace DRLRelease.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (Request.Cookies["UserID"] != null && SessionManager.Instance.UserName != null && Session.Count != 0)
            {
                return RedirectToAction("Index", "Release");
            }
            WebFormsAuthCookies.RemoveAuthCookie("UserID");
            Session.Abandon();
            Session.Clear();
            List<Web_Users> list = CommonDataAccess.GetWebUserList();
            if (list == null || list.Count == 0)
            {
                return RedirectToAction("Register", "Account");
            }
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string Password = CommonFunction.Encrypt(Convert.ToString(model.Password));
                string UserName = Convert.ToString(model.UserName);
                Web_Users user = AccountDataAccess.Login(UserName, Password);

                if (user != null)
                {
                    HttpCookie myCookie = new HttpCookie("myCookie");
                    HttpCookie SetCookie = new HttpCookie("SetCookie");
                    bool IsRemember = model.RememberMe;


                    if (IsRemember)
                    {
                        myCookie.Values.Add("username", UserName);
                        myCookie.Values.Add("password", Convert.ToString(model.Password));
                        myCookie.Expires = DateTime.Now.AddDays(15);

                        SetCookie.Values.Add("username", UserName);
                        SetCookie.Values.Add("password", Convert.ToString(model.Password));
                        SetCookie.Values.Add("remember", "true");
                        SetCookie.Expires = DateTime.Now.AddDays(15);
                    }
                    else
                    {
                        myCookie.Values.Add("username", string.Empty);
                        myCookie.Values.Add("password", string.Empty);
                        myCookie.Expires = DateTime.Now.AddMinutes(30);

                        SetCookie.Values.Add("username", string.Empty);
                        SetCookie.Values.Add("password", string.Empty);
                        SetCookie.Values.Add("remember", "false");
                        SetCookie.Expires = DateTime.Now.AddMinutes(30);
                    }
                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(SetCookie);
                    WebFormsAuthCookies.SetAuthCookie(user.UserId.ToString(), "UserID");
                    WebFormsAuthCookies.SetAuthCookie(user.UserName, "UserName");
                    SessionManager.Instance.UserId = user.UserId;
                    SessionManager.Instance.UserName = user.UserName;
                    SessionManager.Instance.UserEmail = user.EmailAddress;
                    SessionManager.Instance.UserType = user.UserType;
                    return RedirectToLocal(returnUrl);
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebFormsAuthCookies.RemoveAuthCookie("UserID");
            WebFormsAuthCookies.RemoveAuthCookie("UserName");
            Request.Cookies["UserID"].Value = null;
            SessionManager.Instance.UserName = null;
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index", "Release");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            List<Web_Users> list = CommonDataAccess.GetWebUserList();
            if (list != null && list.Count > 0)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    string Password = CommonFunction.Encrypt(Convert.ToString(model.Password));
                    Web_Users webuser = new Web_Users();
                    webuser.EmailAddress = model.EmailAddress;
                    webuser.IsActive = true;
                    webuser.Password = Password;
                    webuser.UserName = model.UserName;
                    webuser.UserType = (int)EnumWebUserType.Admin;
                    if (CommonDataAccess.RegisterNewUser(webuser))
                    {
                        Web_Users user = AccountDataAccess.Login(Convert.ToString(model.UserName), Password);
                        HttpCookie myCookie = new HttpCookie("myCookie");
                        HttpCookie SetCookie = new HttpCookie("SetCookie");

                        myCookie.Values.Add("username", string.Empty);
                        myCookie.Values.Add("password", string.Empty);
                        myCookie.Expires = DateTime.Now.AddMinutes(30);

                        SetCookie.Values.Add("username", string.Empty);
                        SetCookie.Values.Add("password", string.Empty);
                        SetCookie.Values.Add("remember", "false");
                        SetCookie.Expires = DateTime.Now.AddMinutes(30);

                        Response.Cookies.Add(myCookie);
                        Response.Cookies.Add(SetCookie);
                        WebFormsAuthCookies.SetAuthCookie(user.UserId.ToString(), "UserID");
                        WebFormsAuthCookies.SetAuthCookie(webuser.UserName, "UserName");
                        SessionManager.Instance.UserId = user.UserId;
                        SessionManager.Instance.UserName = webuser.UserName;
                        SessionManager.Instance.UserEmail = webuser.EmailAddress;
                        SessionManager.Instance.UserType = webuser.UserType;
                        return RedirectToAction("Index", "Release");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Admin user not registerd.");
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

      
        //
        // GET: /Account/Manage

        // POST: /Account/Manage

      
        //
        // POST: /Account/ExternalLogin

        //
        // POST: /Account/ExternalLoginConfirmation

        //
        // GET: /Account/ExternalLoginFailure

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Release");
            }
        }



        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }

    public class WebFormsAuthCookies
    {
        internal static void SetAuthCookie(string EmpId, string key)
        {
            RemoveAuthCookie(key);
            HttpCookie cookieEmailId = new HttpCookie(key);
            cookieEmailId.Value = EmpId;
            cookieEmailId.Expires = DateTime.Now.AddHours(8);
            HttpContext.Current.Response.Cookies.Add(cookieEmailId);
        }
        internal static void RemoveAuthCookie(string key)
        {
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                HttpCookie cookieEmailId = null;
                cookieEmailId = HttpContext.Current.Request.Cookies[key];
                cookieEmailId.Expires = DateTime.Now.AddHours(-1);
                HttpContext.Current.Response.Cookies.Add(cookieEmailId);
            }

        }
    }
}
