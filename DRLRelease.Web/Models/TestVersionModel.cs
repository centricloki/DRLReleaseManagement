using DRLRelease.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DRLRelease.Web.Models
{
    public class TestVersionModel
    {
        public List<UserAppVersion> VersionList { get; set; }
        public string LatestVersion { get; set; }
    }

    public class TestNotificationModel
    {
        public string DeviceId { get; set; }
        public string Message { get; set; }
    }
}