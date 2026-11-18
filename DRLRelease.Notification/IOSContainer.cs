using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Notification
{
    public class IOSContainer
    {
        public List<string> DeviceIds { get; set; }
        public byte[] CertificateFileBytes { get; set; }
        //public string CertificateFilePath { get; set; }
        public string CertificatePassword { get; set; }
        public int Badge { get; set; }
        public string Sound { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> KeyValues { get; set; }
        public bool Production { get; set; }
    }
}
