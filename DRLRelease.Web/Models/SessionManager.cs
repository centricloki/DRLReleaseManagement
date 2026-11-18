using DRLRelease.Common;
using DRLRelease.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DRLRelease.Web.Models
{

    [Serializable]
    public class SessionManager
    {
        private SessionManager()
        {
        }

        private static readonly SessionManager _instance = new SessionManager();

        public static SessionManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Just for an example
        /// Usage of this property from outside : SessionManager.Instance.UserName
        /// </summary>
        public string UserName
        {
            get
            {
                return HttpContext.Current.Session["SESSION_USERNAME"] != null
                           ? HttpContext.Current.Session["SESSION_USERNAME"].ToString()
                           : null;
            }
            set
            {
                HttpContext.Current.Session["SESSION_USERNAME"] = value;
            }
        }

        public int UserId
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["SESSION_USERID"]) != null ? Convert.ToInt32(HttpContext.Current.Session["SESSION_USERID"]) : 0;
            }
            set
            {
                HttpContext.Current.Session["SESSION_USERID"] = value;
            }
        }
        public string UserEmail
        {
            get
            {
                return HttpContext.Current.Session["SESSION_USEREMAIL"] != null
                                          ? HttpContext.Current.Session["SESSION_USEREMAIL"].ToString()
                                          : null;
            }
            set
            {
                HttpContext.Current.Session["SESSION_USEREMAIL"] = value;
            }
        }

        public int UserType
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["SESSION_USERTYPE"]) != null ? Convert.ToInt32(HttpContext.Current.Session["SESSION_USERTYPE"]) : 0;
            }
            set
            {
                HttpContext.Current.Session["SESSION_USERTYPE"] = value;
            }
        }
    }

}