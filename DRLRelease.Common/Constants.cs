using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Common
{
    public static class Constants
    {
        #region "For Pushnotification"
        public static string PushNotificationCertificatePath { get; set; }
        public static string PushNotificationCertificatePassword { get; set; }
        public static string PushNotificationMessage { get; set; }

        public static string IPadApp { get; set; }
        #endregion "For Pushnotification"

        //public static string WebServiceUrl { get; set; }
        public static int WebRequestLength { get; set; }
    }
}
