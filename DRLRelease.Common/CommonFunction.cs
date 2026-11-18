using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Common
{
    public class CommonFunction
    {
        public static string DirectoryPath = Environment.CurrentDirectory;
        public static string Decrypt(string Value)
        {
            byte[] key = System.Text.Encoding.UTF8.GetBytes("@#-DRL!^");
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            if (Value.Trim().Length > 0)
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream
                        (Convert.FromBase64String(Value));
                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    cryptoProvider.CreateDecryptor(key, IV), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptoStream);
                return reader.ReadToEnd();
            }
            return Value;
        }
        public static string Encrypt(string Value)
        {
            try
            {
                byte[] key = System.Text.Encoding.UTF8.GetBytes("@#-DRL!^");
                byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
                if (Value.Trim().Length > 0)
                {
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(Value);
                    MemoryStream ms = new MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
                return Value;
            }
            catch
            {
                return Value;
            }
        }
        public static string GetConfigurationValue(string key)
        {
            string _returnValue = "";

            if (System.Configuration.ConfigurationManager.AppSettings[key] != null && System.Configuration.ConfigurationManager.AppSettings[key].ToString().Length > 0)
            {
                _returnValue = System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            }

            return _returnValue;
        }
        public static int GetConfigurationValueInt(string key)
        {
            string _returnValue = "";

            if (System.Configuration.ConfigurationManager.AppSettings[key] != null && System.Configuration.ConfigurationManager.AppSettings[key].ToString().Length > 0)
            {
                _returnValue = System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(_returnValue))
                    return Convert.ToInt32(_returnValue);
            }
            catch { }

            return 300;
        }
    
    }

   
}
