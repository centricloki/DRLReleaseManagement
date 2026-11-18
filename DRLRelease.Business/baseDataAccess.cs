using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DRLRelease.Business
{
    public class baseDataAccess
    {
        //public static string connectionString = System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]; //ConfigurationManag//er.ConnectionStrings["MosoClub"].ConnectionString; 
        public static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; //ConfigurationManager.ConnectionStrings["MosoClub"].ConnectionString; 
    }
}
