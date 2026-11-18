using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Notification
{
    public class PushResults
    {
        public PushResults()
        {
            FailedDeviceTokens = new List<string>();
            SuccessDeviceTokens = new List<string>();
        }
        public List<string> FailedDeviceTokens { get; set; }
        public List<string> SuccessDeviceTokens { get; set; }
    }
}
