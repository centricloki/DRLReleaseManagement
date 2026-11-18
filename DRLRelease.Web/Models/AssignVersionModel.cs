using DRLRelease.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DRLRelease.Web.Models
{
    public class AssignVersionModel
    {
        public string VersionType { get; set; }
        public string UserIDs { get; set; }
        public string UserNames { get; set; }
        public string CurrentVersion { get; set; }
        public List<AppVersion> AssignVersionList { get; set; }
        public SelectList AssignList { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsDisabledButton { get; set; }
        public int CurrentVersionId { get; set; }
    }
}