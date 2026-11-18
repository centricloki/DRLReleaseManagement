using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace DRLRelease.DataModel
{
    public class DataAccessHelper
    {
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            try
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName.ToLower())
                    .ToList();

                var properties = typeof(T).GetProperties();

                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();

                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                            pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : row[pro.Name], null);
                    }

                    return objT;
                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static IEnumerable<T> ConvertToEnumerable<T>(DataTable dt)
        {
            try
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName.ToLower())
                    .ToList();

                var properties = typeof(T).GetProperties();

                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();

                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                            pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : row[pro.Name], null);
                    }

                    return objT;
                }).AsEnumerable();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string CheckEscape(string variable)
        {
            return (!string.IsNullOrEmpty(variable) ? variable.Replace("'", "''") : variable);
        }

        public static string CheckHTMLEscape(string variable)
        {
            return (!string.IsNullOrEmpty(variable) ? variable.Replace("'", "&#39;") : variable);
        }
    }
}
