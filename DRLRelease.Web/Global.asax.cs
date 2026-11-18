using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DRLRelease.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            // Code that runs on application startup
            Common.CommonFunction.DirectoryPath = System.Web.HttpContext.Current.Server.MapPath("~");
            Common.Constants.PushNotificationCertificatePath = string.Format("{0}{1}", HttpRuntime.AppDomainAppPath, Common.CommonFunction.GetConfigurationValue("PushNotificationCertificatePath"));
            Common.Constants.PushNotificationCertificatePassword = Common.CommonFunction.GetConfigurationValue("PushNotificationCertificatePassword");
            Common.Constants.PushNotificationMessage = Common.CommonFunction.GetConfigurationValue("PushNotificationMessage");

            Common.Constants.IPadApp = string.Format("{0}{1}", HttpRuntime.AppDomainAppPath, Common.CommonFunction.GetConfigurationValue("UploadedIPadApp"));
            //Common.Constants.WebServiceUrl = Common.CommonFunction.GetConfigurationValue("WebServiceUrl");

            Common.Constants.WebRequestLength = Common.CommonFunction.GetConfigurationValueInt("WebRequestLength");

        }
    }
}