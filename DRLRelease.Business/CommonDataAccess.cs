using DRLRelease.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Business
{
    public class CommonDataAccess : baseDataAccess
    {
        public static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; //ConfigurationManager.ConnectionStrings["MosoClub"].ConnectionString; 

        public static bool AddNewAppVersion(AppVersion appversion)
        {
            bool result = false;
            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@AppVersionID", appversion.AppVersionID);
            parameters[1] = new SqlParameter("@Version", appversion.Version);
            parameters[2] = new SqlParameter("@DownloadURL", appversion.DownloadURL);
            parameters[3] = new SqlParameter("@IsProduction", appversion.IsProduction);
            parameters[4] = new SqlParameter("@IsActive", appversion.IsActive);
            parameters[5] = new SqlParameter("@WhatsNew", appversion.WhatsNew);

            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "sp_Release_AddAppVersion", parameters);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static List<AppVersion> GetAppVersion(AppVersion appversion)
        {
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@AppVersionID", appversion.AppVersionID);
            parameters[1] = new SqlParameter("@IsProduction", appversion.IsProduction);

            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "sp_Release_GetAppVersion", parameters);
            List<AppVersion> lstP = new List<AppVersion>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                lstP = DataAccessHelper.ConvertToList<AppVersion>(_dtTable);
            }
            return lstP;
        }

        public static List<AppVersion> GetAppVersionList()
        {
            SqlParameter[] parameters = new SqlParameter[0];
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "SELECT * FROM AppVersion where IsActive = 1 AND AppVersionID >= ( SELECT AppVersionID FROM AppVersion WHERE IsProduction = 1 )", parameters);
            List<AppVersion> lstP = new List<AppVersion>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                lstP = DataAccessHelper.ConvertToList<AppVersion>(_dtTable);
            }
            return lstP;
        }

        public static bool SetNotificationToUser(long UserID)
        {
            int result = SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, string.Format("Update UserApplicationDetails set IsSendNotification=1 WHERE UserID = {0}", UserID), null);
            if (result != -1)
            {
                return true;
            }
            return false;
        }

        public static string GetUserDeviceToken(long userID)
        {
            SqlParameter[] parameters = new SqlParameter[0];
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, string.Format("SELECT DeviceToken FROM UserApplicationDetails WHERE UserID = {0}", userID), parameters);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["DeviceToken"] != null ? ds.Tables[0].Rows[0]["DeviceToken"].ToString() : null;
            }
            return null;
        }

        public static List<AppVersion> GetAllVersionList()
        {
            SqlParameter[] parameters = new SqlParameter[0];
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "SELECT * FROM AppVersion", parameters);
            List<AppVersion> lstP = new List<AppVersion>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                lstP = DataAccessHelper.ConvertToList<AppVersion>(_dtTable);
            }
            return lstP;
        }

        public static AppVersion GetAppVersion(string version)
        {
            SqlParameter[] parameters = new SqlParameter[0];
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, string.Format("SELECT * FROM AppVersion WHERE Version = '{0}'", version), parameters);
            AppVersion app = null;
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                app = DataAccessHelper.ConvertToList<AppVersion>(_dtTable).FirstOrDefault();
            }
            return app;
        }

        public static List<AppVersion> GetVersionList()
        {
            SqlParameter[] parameters = new SqlParameter[0];
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "SELECT *, CONVERT(BIT,ISNULL(( SELECT 0 FROM AppVersion WHERE AppVersionID >= app.AppVersionID AND IsProduction = 1),1))  AS IsAction FROM AppVersion app", parameters);
            List<AppVersion> lstP = new List<AppVersion>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                lstP = DataAccessHelper.ConvertToList<AppVersion>(_dtTable);
            }
            return lstP;
        }

        public static List<UserAppVersion> GetUserAppVersionList(string VersionType)
        {
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@VersionType", VersionType);
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "sp_Release_GetUserAppVersionList", parameters);
            List<UserAppVersion> lstP = new List<UserAppVersion>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                lstP = DataAccessHelper.ConvertToList<UserAppVersion>(_dtTable);
            }
            return lstP;
        }

        public static bool SetUserAppVersion(UserAppVersion userappversion)
        {
            bool result = false;
            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@UserID", userappversion.UserID);
            parameters[1] = new SqlParameter("@IsSendNotification", userappversion.IsSendNotification);
            parameters[2] = new SqlParameter("@NotificationCode", userappversion.NotificationCode);
            parameters[3] = new SqlParameter("@DeviceID", userappversion.DeviceID);
            parameters[4] = new SqlParameter("@CurrentVersion", userappversion.CurrentVersion);
            parameters[5] = new SqlParameter("@AssignedVersion", userappversion.AssignedVersion);

            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "sp_Release_UserAppVersion", parameters);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool UpdateProductionAppVersion(int AppVersionId, string whatsnew)
        {
            bool result = false;
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@AppVersionID", AppVersionId);
            parameters[1] = new SqlParameter("@WhatsNew", whatsnew);

            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "sp_Release_UpdateProductionVersion", parameters);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static List<Web_Users> GetWebUserList()
        {
            SqlParameter[] parameters = new SqlParameter[0];
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "SELECT * FROM web_users", parameters);
            List<Web_Users> lstP = new List<Web_Users>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                lstP = DataAccessHelper.ConvertToList<Web_Users>(_dtTable);
            }
            return lstP;
        }

        public static bool RegisterNewUser(Web_Users model)
        {
            bool result = false;
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("@UserName", model.UserName);
            parameters[1] = new SqlParameter("@Password", model.Password);
            parameters[2] = new SqlParameter("@EmailAddress", model.EmailAddress);
            parameters[3] = new SqlParameter("@UserType", model.UserType);
            parameters[4] = new SqlParameter("@IsActive", model.IsActive);

            string query = "INSERT INTO web_users (UserName,Password,EmailAddress,UserType,IsActive) VALUES(@UserName,@Password,@EmailAddress,@UserType,@IsActive)"
                         + "SELECT * from web_users";

            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, query, parameters);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool UpdateAppVersionStatus(int aapVersionId, int status)
        {
            string strSQL = string.Format("Update AppVersion SET IsActive = {1} WHERE AppVersionID = {0}", aapVersionId, status);
            int result = SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, strSQL, null);
            if (result != -1)
            {
                return true;
            }
            return false;
        }
    }
}
