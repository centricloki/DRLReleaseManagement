using DRLRelease.Business;
using DRLRelease.Common;
using DRLRelease.DataModel;
using DRLRelease.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Configuration;
using System.Web.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using DRLRelease.Notification;

namespace DRLRelease.Web.Controllers
{
    [AuthorizeWebForm]
    public class ReleaseController : Controller
    {
        //
        // GET: /Release/
        public ActionResult Index()
        {
            //Production Version
            ViewBag.LatestProductionVersion = "V_0.0";
            List<AppVersion> appversions = CommonDataAccess.GetAppVersionList();
            AppVersion app = (from ap in appversions where ap.IsActive == true && ap.IsProduction == true select ap).FirstOrDefault();
            if (app != null)
                ViewBag.LatestProductionVersion = "V_" + app.Version.ToString();
            //Test Version
            ViewBag.LatestTestVersion = "V_0.0";
            List<AppVersion> testapp = (from ap in appversions where ap.IsActive == true && ap.IsProduction == false select ap).ToList();
            string TestVersion = "";
            foreach (AppVersion item in testapp)
            {
                if (!string.IsNullOrEmpty(TestVersion))
                    TestVersion = TestVersion + " , V_" + Convert.ToString(item.Version);
                else
                    TestVersion = " V_" + Convert.ToString(item.Version);
            }
            if (!string.IsNullOrEmpty(TestVersion))
                ViewBag.LatestTestVersion = TestVersion;
            // Active Version
            ViewBag.ActiveVersions = "V_0.0";
            List<AppVersion> allapp = (from ap in appversions where ap.IsActive == true select ap).ToList();
            string ActiveVersion = "";
            foreach (AppVersion item in allapp)
            {
                if (!string.IsNullOrEmpty(ActiveVersion))
                    ActiveVersion = ActiveVersion + " , V_" + Convert.ToString(item.Version);
                else
                    ActiveVersion = " V_" + Convert.ToString(item.Version);
            }
            if (!string.IsNullOrEmpty(ActiveVersion))
                ViewBag.ActiveVersions = ActiveVersion;

            return View();
        }

        #region Production List
        // below action is used to bind partial view which is not using now
        public ActionResult ProductionVersionList()
        {
            ProductionVersionsModel model = new ProductionVersionsModel();
            model.VersionList = CommonDataAccess.GetUserAppVersionList("Production");
            return PartialView("_ProductionVersionsList", model);
        }
        [HttpPost]
        public JsonResult ProductionList()
        {
            List<UserAppVersion> list = CommonDataAccess.GetUserAppVersionList("Production");
            return Json(list);
        }

        #endregion

        #region Test Version List

        [HttpPost]
        public JsonResult TestList()
        {
            List<UserAppVersion> list = CommonDataAccess.GetUserAppVersionList("Test");
            return Json(list);
        }
        // below action is used to bind partial view which is not using now
        public ActionResult TestVersionList()
        {
            TestVersionModel model = new TestVersionModel();
            model.VersionList = CommonDataAccess.GetUserAppVersionList("Test");
            return PartialView("_TestVersionsList", model);
        }

        #endregion

        #region All UserList
        [HttpPost]
        public JsonResult AllUserList()
        {
            List<UserAppVersion> list = CommonDataAccess.GetUserAppVersionList("All");
            return Json(list);
        }
        #endregion

        #region Upload New App

        public ActionResult UploadNewApp()
        {
            UploadNewAppModel model = new UploadNewAppModel();
            HttpRuntimeSection section = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
            double maxFileSize = Math.Round(section.MaxRequestLength / 1024.0, 1);
            string FileSizeLimit = string.Format("Note : Make sure your file is under {0:0.#} MB.", maxFileSize);
            model.FileSizeNote = FileSizeLimit;
            //return PartialView("Popup_UploadNewApp", model);
            return View("UploadNewApp", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadNewApp(UploadNewAppModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                bool NeedToUpload = true, NeedToShow = false;
                string Message = string.Empty;
                try
                {
                    if (Request.Files.Count > 0)
                    {
                        HttpFileCollectionBase files = Request.Files;
                        if (files.Count == 2)
                        {
                            HttpPostedFileBase fileUpIPA = files[0];
                            HttpPostedFileBase fileUpPLIST = files[1];
                            if (fileUpIPA.ContentLength > 0 && fileUpPLIST.ContentLength > 0)
                            {
                                if (!CheckFileType(Path.GetExtension(fileUpIPA.FileName), AppFileType.IPA))
                                    NeedToUpload = false;

                                if (!CheckFileType(Path.GetExtension(fileUpPLIST.FileName), AppFileType.PLIST))
                                    NeedToUpload = false;

                                if (!NeedToUpload)
                                    Message = "Either <b>IPA</b> or <b>PLIST</b> file(s) has selected wrong type of file(s).";

                                if (!Directory.Exists(Constants.IPadApp))
                                    Directory.CreateDirectory(Constants.IPadApp);

                                string filePath = string.Format("{0}\\{1}", Constants.IPadApp, Guid.NewGuid());

                                if (Directory.Exists(filePath))
                                    Directory.Delete(filePath, true);

                                Directory.CreateDirectory(filePath);

                                string ipa = string.Format("{0}\\{1}", filePath, Path.GetFileName(fileUpIPA.FileName));
                                string plist = string.Format("{0}\\{1}", filePath, Path.GetFileName(fileUpPLIST.FileName));

                                fileUpIPA.SaveAs(ipa);
                                fileUpPLIST.SaveAs(plist);

                                UploadedVersion version = GetIpadVersion(plist);

                                string appUploaded = string.Format("{0}\\{1}", Constants.IPadApp, version.Version);

                                if (Copy(filePath, appUploaded))
                                {
                                    AppVersion appversion = CommonDataAccess.GetAppVersion(version.Version);
                                    
                                    if (appversion == null) { appversion = new AppVersion(); }
                                    
                                    appversion.DownloadURL = version.DownloadURL.Replace(".ipa", ".plist").Trim();
                                    appversion.Version = version.Version;
                                    appversion.IsActive = true;
                                    appversion.IsProduction = false;
                                    
                                    if (!CommonDataAccess.AddNewAppVersion(appversion))
                                    {
                                        Message = "There will be some issue while uploading files.";
                                        NeedToShow = true;
                                    }
                                }
                                else
                                {
                                    Message = "Either <b>IPA</b> or <b>PLIST</b> file are missing. Try to upload again.";
                                    NeedToShow = true;
                                }
                            }
                            else
                            {
                                Message = "Either <b>IPA</b> or <b>PLIST</b> file are missing. Try to upload again.";
                                NeedToShow = true;
                            }
                        }
                        else
                        {
                            Message = "Either <b>IPA</b> or <b>PLIST</b> file are missing. Try to upload again.";
                            NeedToShow = true;
                        }

                    }
                    if (!string.IsNullOrWhiteSpace(Message))
                    {
                        ModelState.AddModelError("", Message);
                    }
                    else
                    {
                        ViewBag.result = "New Application has been succfully uploaded!";
                    }
                    return View(model);
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex);
                    ModelState.AddModelError("", "Upload failed");
                    return View(model);
                }
                //ModelState.AddModelError("", Message);
            }
            return View(model);
        }
        #endregion

        #region Upload Production Version

        public ActionResult PopupUploadProductionVersion()
        {
            UploadProductionVersionModel model = new UploadProductionVersionModel();
            AppVersion data = new AppVersion();
            data.AppVersionID = 0;
            data.IsProduction = false;

            List<AppVersion> list = CommonDataAccess.GetAppVersion(data);
            list = list.Where(x => x.IsActive == true).ToList();
            model.TestVersionList = list;

            return PartialView("Popup_UploadProductionVersion", model);
        }

        public ActionResult UpdateProductionVersion(int AppVersionId, string[] WhatsNewList)
        {
            bool IsSuccess = false;
            string Message = string.Empty;
            try
            {
                string strwhatsnew = (WhatsNewList != null && WhatsNewList.Length > 0) ? string.Join(".", WhatsNewList) : "";
                IsSuccess = CommonDataAccess.UpdateProductionAppVersion(AppVersionId, strwhatsnew);
                if (!IsSuccess)
                {
                    Message = "Production app version NOT updated.";
                }
            }
            catch (Exception ex)
            {
                Message = "Error accord while updating production app";
                IsSuccess = false;
                ExceptionLog.WriteLog(ex);
            }
            return Json(new { success = IsSuccess, message = Message });
        }

        #endregion

        #region Production and Version Filters

        public ActionResult VersionListWithFilters(UserAppVersion filermodel)
        {
            ProductionVersionsModel model = new ProductionVersionsModel();
            List<UserAppVersion> list = new List<UserAppVersion>();
            if (Convert.ToString(filermodel.VersionType) == "Production")
            {
                list = CommonDataAccess.GetUserAppVersionList("Production");
            }
            else if (Convert.ToString(filermodel.VersionType) == "All")
            {
                list = CommonDataAccess.GetUserAppVersionList("All");
            }
            else
            {
                list = CommonDataAccess.GetUserAppVersionList("Test");
            }
            if (filermodel != null)
            {
                if (filermodel.UserID > 0)
                {
                    list = list.Where(x => x.UserID == filermodel.UserID).ToList();
                }
                if (!string.IsNullOrWhiteSpace(Convert.ToString(filermodel.UserName)))
                {
                    list = list.Where(x => x.FirstName == filermodel.UserName).ToList();
                }
                if (!string.IsNullOrWhiteSpace(Convert.ToString(filermodel.CurrentVersionName)))
                {
                    list = list.Where(x => x.CurrentVersionName == filermodel.CurrentVersionName).ToList();
                }
                if (Convert.ToString(filermodel.IsUpdatedFilter) != "All")
                {
                    switch (Convert.ToString(filermodel.IsUpdatedFilter))
                    {
                        case "No":
                            list = list.Where(x => x.IsUpdated == false).ToList();
                            break;
                        case "Yes":
                            list = list.Where(x => x.IsUpdated == true).ToList();
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(Convert.ToString(filermodel.IOSVersion)))
                {
                    list = list.Where(x => x.IOSVersion == filermodel.IOSVersion).ToList();
                }

            }
            model.VersionList = list;
            return PartialView("_ProductionVersionsList", model);
        }

        public ActionResult PopupProductionFilter(string VersionType)
        {
            ProductionFilterModel model = new ProductionFilterModel();
            model.VersionType = VersionType;
            return PartialView("Popup_ProductionFilter", model);
        }
        #endregion

        #region AssignVersion

        public ActionResult PopupAssignVersion(string VersionType, string[] UserIdList, string[] UserNameList, string[] UserCurrentVersions, string[] CurrentVersionIdList)
        {
            AssignVersionModel model = new AssignVersionModel();
            model.VersionType = VersionType;
            model.ErrorMessage = "";
            model.CurrentVersion = "";
            model.CurrentVersionId = 0;
            string UserIdlist = "";
            string UserNames = "";
            List<AppVersion> AssignVersions = new List<AppVersion>();
            if (UserIdList != null && UserIdList.Count() > 0)
            {
                foreach (var item in UserIdList)
                {
                    if (UserIdlist == "")
                        UserIdlist = item;
                    else
                        UserIdlist = UserIdlist + " / " + item;
                }
                foreach (var item in UserNameList)
                {
                    if (UserNames == "")
                        UserNames = item;
                    else
                        UserNames = UserNames + " / " + item;
                }
                //Check Current Version is same to all user or not

                if (UserCurrentVersions.Length > 0)
                {
                    string[] currentVersionlist = UserCurrentVersions.Select(x => x).Distinct().ToArray();
                    if (currentVersionlist.Length > 1)
                    {
                        model.IsDisabledButton = true;
                        model.ErrorMessage = "Selected User has different Current Version";
                    }
                    else
                    {
                        model.CurrentVersion = Convert.ToString(currentVersionlist[0]);
                        model.CurrentVersionId = Convert.ToInt32(CurrentVersionIdList[0]);
                    }
                }
                AssignVersions = CommonDataAccess.GetAppVersionList().Where(x => x.IsProduction == true || x.IsActive == true).ToList();
            }
            else
            {
                model.IsDisabledButton = true;
                model.ErrorMessage = "No Record Selected";
            }

            model.UserIDs = UserIdlist;
            model.UserNames = UserNames;
            model.AssignList = new SelectList(AssignVersions, "AppVersionID", "Version");
            if (model.ErrorMessage == "" && AssignVersions.ToList().Count == 0)
            {
                model.IsDisabledButton = true;
                model.ErrorMessage = "No new vesion is available to assing this user";
            }
            return PartialView("Popup_AssignVersion", model);
        }

        public ActionResult SaveAssignVersion(int AssingedVersionId, string[] UserIdList, int UserCurrentVersionId)
        {
            bool IsSuccessful = false;
            string message = string.Empty;
            try
            {
                foreach (var item in UserIdList)
                {
                    UserAppVersion data = new UserAppVersion();
                    data.AssignedVersion = AssingedVersionId;
                    data.CurrentVersion = UserCurrentVersionId;
                    data.NotificationCode = "";
                    data.UserID = Convert.ToInt32(item);
                    bool result = CommonDataAccess.SetUserAppVersion(data);
                    if (result)
                    {
                        string deviceToken = CommonDataAccess.GetUserDeviceToken(data.UserID);
                        if (!string.IsNullOrWhiteSpace(deviceToken) && Convert.ToBoolean(Common.CommonFunction.GetConfigurationValue("IsSendPushNotification")))
                        {
                            var Assignedversiondata = CommonDataAccess.GetAppVersionList().Where(x => x.AppVersionID == AssingedVersionId).ToList().FirstOrDefault().Version;
                            if (Pusher.SendToiOSDevice(deviceToken, "A new " + Assignedversiondata + " version has been assigned to you."))
                            {
                                CommonDataAccess.SetNotificationToUser(data.UserID);
                            }
                        }
                    }
                }
                IsSuccessful = true;
            }
            catch (Exception ex)
            {
                message = "Version not assined to selected user.";
                IsSuccessful = false;
                ExceptionLog.WriteLog(ex);
            }
            return Json(new { Success = IsSuccessful, Message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Versions
        public ActionResult AppVersions()
        {
            return View("AppVersions");
        }
        public ActionResult VersionList()
        {
            List<AppVersion> list = CommonDataAccess.GetVersionList();
            return Json(list);
        }
        #endregion

        #region Method
        /// <summary>
        /// Extract version of IOS .plist file
        /// </summary>
        /// <returns></returns>
        private UploadedVersion GetIpadVersion(string plistfile)
        {
            string downloadUrl = string.Empty;
            string version = string.Empty;
            UploadedVersion ipadDetails = new UploadedVersion();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(plistfile);
                XmlNode downloadURLNode = doc.SelectSingleNode("//dict/array/dict/array/dict/string[2]");
                XmlNode versionNode = doc.SelectSingleNode("//dict/array/dict/dict[1]/string[2]");

                if (downloadURLNode != null)
                    downloadUrl = downloadURLNode.InnerText;
                if (versionNode != null)
                    version = versionNode.InnerText;

                ipadDetails.DownloadURL = downloadUrl;
                ipadDetails.Version = version;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex);
            }

            return ipadDetails;
        }
        /// <summary>
        /// Copy files from source directory to target directory
        /// </summary>
        /// <param name="sourceDir">actual directory contains files</param>
        /// <param name="targetDir">directory where we need to copy files</param>
        /// <returns></returns>
        private bool Copy(string sourceDir, string targetDir)
        {
            try
            {
                if (Directory.Exists(sourceDir))
                {
                    //check target directory exist or not if not then first create it, or if exists then delete it
                    if (Directory.Exists(targetDir))
                        Directory.Delete(targetDir, true);
                    Directory.CreateDirectory(targetDir);

                    //used to copy files from source to target
                    foreach (var file in Directory.GetFiles(sourceDir))
                        System.IO.File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

                    //used to copy directory from source to target
                    //foreach (var directory in Directory.GetDirectories(sourceDir))
                    //    Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));

                    Directory.Delete(sourceDir, true);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex);
                return false;
            }
        }

        /// <summary>
        /// Used to validate uploaded files type
        /// </summary>
        /// <param name="ext">extention of uploaded files</param>
        /// <param name="filetype">enum value to validate uploaded file type</param>
        /// <returns></returns>
        protected bool CheckFileType(string ext, AppFileType filetype)
        {
            if (filetype == AppFileType.IPA && ext.TrimStart('.').ToUpper().Equals(AppFileType.IPA.ToString()))
                return true;

            if (filetype == AppFileType.PLIST && ext.TrimStart('.').ToUpper().Equals(AppFileType.PLIST.ToString()))
                return true;

            return false;
        }

        #endregion

        public ActionResult UpdateAppVersion(int aapVersionId, int status)
        {
            bool IsSuccessful = false;
            string message = string.Empty;
            try
            {
                CommonDataAccess.UpdateAppVersionStatus(aapVersionId, status);
                IsSuccessful = true;
            }
            catch (Exception ex)
            {
                message = "Please try again! something worng with server.";
                IsSuccessful = false;
                ExceptionLog.WriteLog(ex);
            }
            return Json(new { Success = IsSuccessful, Message = message }, JsonRequestBehavior.AllowGet);
        }
    }
}
