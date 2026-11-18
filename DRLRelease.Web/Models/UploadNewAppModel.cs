using DRLRelease.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Extensions;

namespace DRLRelease.Web.Models
{
    public class UploadNewAppModel
    {
        [Required(ErrorMessage = "Please Upload .IPA File")]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase fileUpIPA { get; set; }

        [Required(ErrorMessage = "Please Upload .PLIST File")]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase fileUpPLIST { get; set; }

        public string FileSizeNote { get; set; }
    }


}