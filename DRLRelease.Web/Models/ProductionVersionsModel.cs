using DRLRelease.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DRLRelease.Web.Models
{
    public class ProductionVersionsModel
    {
        public List<UserAppVersion> VersionList { get; set; }
        public string LatestVersion { get; set; }
    }

    public class VersionList
    {

    }
}