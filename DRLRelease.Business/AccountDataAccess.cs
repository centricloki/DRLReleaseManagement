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
    public class AccountDataAccess : baseDataAccess
    {
        public static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; //ConfigurationManager.ConnectionStrings["MosoClub"].ConnectionString; 

        public static Web_Users Login(string UserName, string Password)
        {
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@UserName", UserName);
            parameters[1] = new SqlParameter("@Password", Password);

            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "sp_Release_WebLogin", parameters);
            List<Web_Users> lstP = new List<Web_Users>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                lstP = DataAccessHelper.ConvertToList<Web_Users>(_dtTable);
            }
            return lstP.FirstOrDefault();
        }

        public static Web_Users GetWebUser(string UserName, string Password)
        {
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@UserName", UserName);
            parameters[1] = new SqlParameter("@Password", Password);

            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "sp_Release_WebLogin", parameters);
            List<Web_Users> lstP = new List<Web_Users>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable _dtTable = ds.Tables[0];
                lstP = DataAccessHelper.ConvertToList<Web_Users>(_dtTable);
            }
            return lstP.FirstOrDefault();
        }
    }
}
