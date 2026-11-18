using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Notification
{
    public class PushResponse
    {
        public PushResponse()
        {
            PushResults = new PushResults();
        }

        public bool Status { get; set; }
        public string Message { get; set; }
        public string PushJsonString { get; set; }
        public PushResults PushResults { get; set; }
    }
}
