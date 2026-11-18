using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Notification
{
    public class HelperMethods
    {
        public static string APNSPushJson(IOSContainer container)
        {
            string payload = "{\"aps\":{\"alert\":\"" + container.Message + "\"";

            if (container.Badge != 0)
            {
                payload += ",\"badge\":" + container.Badge;
            }
            if (!String.IsNullOrEmpty(container.Sound))
            {
                payload += ",\"sound\":\"" + container.Sound + "\"";
            }


            payload += "}";


            if (container.KeyValues != null)
            {
                foreach (var pair in container.KeyValues)
                {
                    if (!String.IsNullOrEmpty(pair.Key))
                    {
                        payload += ", \"" + pair.Key + "\":\"" + pair.Value + "\"";
                    }
                }
            }

            payload += "}";

            return payload;
        }
    }
}
