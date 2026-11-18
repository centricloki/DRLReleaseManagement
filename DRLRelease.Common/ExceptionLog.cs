using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Common
{
    public class ExceptionLog
    {
        public ExceptionLog()
        {
        }

        public static void WriteLog(Exception e)
        {
            try
            {
                string CompDirectoryPath = CommonFunction.DirectoryPath + "\\Error";
                //string CompDirectoryPath = System.IO.Path.GetTempPath() + "MFSSWebError";
                string FileName = string.Format("{0}_{1}_{2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                string FilePath = CompDirectoryPath + "\\" + FileName;
                if (!System.IO.Directory.Exists(CompDirectoryPath))
                    System.IO.Directory.CreateDirectory(CompDirectoryPath);
                System.IO.StreamWriter streamwriter;
                if (!System.IO.File.Exists(FilePath))
                    streamwriter = System.IO.File.CreateText(FilePath);
                else
                    streamwriter = new System.IO.StreamWriter(FilePath, true);

                streamwriter.WriteLine(DateTime.Now.ToString());
                streamwriter.WriteLine(e.Source);
                streamwriter.WriteLine(e.Message);
                streamwriter.WriteLine(e.TargetSite);
                streamwriter.WriteLine(e.StackTrace);
                if (e.InnerException != null)
                    streamwriter.WriteLine("[" + e.InnerException.Message + "]" + e.InnerException.StackTrace);
                streamwriter.WriteLine(string.Empty);
                streamwriter.Close();
            }
            catch
            {
                System.Diagnostics.EventLog.WriteEntry(e.Source, e.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }
        public static void WriteLog(string _errorMsg)
        {
            try
            {
                string CompDirectoryPath = CommonFunction.DirectoryPath + "\\Error";
                //string CompDirectoryPath = System.IO.Path.GetTempPath() + "MFSSWebError";
                string FileName = string.Format("{0}_{1}_{2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                string FilePath = CompDirectoryPath + "\\" + FileName;
                if (!System.IO.Directory.Exists(CompDirectoryPath))
                    System.IO.Directory.CreateDirectory(CompDirectoryPath);
                System.IO.StreamWriter streamwriter;
                if (!System.IO.File.Exists(FilePath))
                    streamwriter = System.IO.File.CreateText(FilePath);
                else
                    streamwriter = new System.IO.StreamWriter(FilePath, true);

                streamwriter.WriteLine(DateTime.Now.ToString());
                streamwriter.WriteLine(_errorMsg);
                streamwriter.WriteLine(string.Empty);
                streamwriter.Close();
            }
            catch
            {
                System.Diagnostics.EventLog.WriteEntry("PushNotification", _errorMsg, System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}
