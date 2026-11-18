using Newtonsoft.Json;
using PushSharp.Core;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PushSharp.Google;

namespace DRLRelease.Notification
{
    public static class Pusher
    {
        private static PushResponse SendToiOS(IOSContainer container)
        {
            string payload = string.Empty;
            PushResponse response = new PushResponse();

            try
            {
                // Configuration (NOTE: .pfx can also be used here)
                var config = new ApnsConfiguration(container.Production ? ApnsConfiguration.ApnsServerEnvironment.Production : ApnsConfiguration.ApnsServerEnvironment.Sandbox,
                    container.CertificateFileBytes, container.CertificatePassword);

                // Create a new broker
                var apnsBroker = new ApnsServiceBroker(config);

                // Wire up events
                apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
                {

                    aggregateEx.Handle(ex =>
                    {

                        // See what kind of exception it was to further diagnose
                        if (ex is ApnsNotificationException)
                        {
                            var notificationException = (ApnsNotificationException)ex;

                            // Deal with the failed notification
                            var apnsNotification = notificationException.Notification;
                            var statusCode = notificationException.ErrorStatusCode;

                            //Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");
                            response.PushResults.FailedDeviceTokens.Add(apnsNotification.DeviceToken);
                        }
                        else
                        {
                            // Inner exception might hold more useful information like an ApnsConnectionException           
                            //Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}");
                        }

                        // Mark it as handled
                        return true;
                    });
                };

                apnsBroker.OnNotificationSucceeded += (notification) =>
                {
                    Console.WriteLine("Apple Notification Sent!");
                    response.PushResults.SuccessDeviceTokens.Add(notification.DeviceToken);
                };

                // Start the broker
                apnsBroker.Start();


                payload = HelperMethods.APNSPushJson(container);


                foreach (var deviceToken in container.DeviceIds)
                {
                    // Queue a notification to send
                    apnsBroker.QueueNotification(new ApnsNotification
                    {
                        DeviceToken = deviceToken,
                        Payload = JObject.Parse(payload)
                    });
                }

                // Stop the broker, wait for it to finish   
                // This isn't done after every message, but after you're
                // done with the broker
                apnsBroker.Stop();

                response.Status = true;
                response.Message = "Apple Notification Sent Successfully";
                response.PushJsonString = Convert.ToString(payload);
                return response;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Convert.ToString(ex);
                response.PushJsonString = Convert.ToString(payload);
                return response;
            }
        }

        public static bool SendToiOSDevice(string deviceToken, string message)
        {
            List<string> deviceIds = new List<string>();
            deviceIds.Add(deviceToken);
            //string certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\US_prod_Cert.p12");
            IOSContainer iContainer = new IOSContainer()
            {
                DeviceIds = deviceIds,
                Message = message,
                CertificateFileBytes = File.ReadAllBytes(Common.Constants.PushNotificationCertificatePath),
                Production = true,
                CertificatePassword = Common.Constants.PushNotificationCertificatePassword,
                Badge = 1,
                Sound = "default",
            };
            var res = SendToiOS(iContainer);
            return res != null ? res.Status : false;
        }
    }
}
