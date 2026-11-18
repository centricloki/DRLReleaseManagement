using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLRelease.Notification
{
    public class SendNotification
    {
        //PushBroker push = new PushBroker();

        public SendNotification()
        {
            //push.OnNotificationSent += NotificationSent;
            //push.OnChannelException += ChannelException;
            //push.OnServiceException += ServiceException;
            //push.OnNotificationFailed += NotificationFailed;
            //push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            //push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            //push.OnChannelCreated += ChannelCreated;
            //push.OnChannelDestroyed += ChannelDestroyed;
        }

        public static bool PushNotificationApple(string strDeviceToken, string strMessage)
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(strMessage))
                //{
                //    strMessage = Common.Constants.PushNotificationMessage;
                //}
                //PushBroker push = new PushBroker();
                //var appleCert = System.IO.File.ReadAllBytes(Common.Constants.PushNotificationCertificatePath);
                //push.RegisterAppleService(new ApplePushChannelSettings(appleCert, Common.Constants.PushNotificationCertificatePassword));
                //push.QueueNotification(new AppleNotification()
                //                           .ForDeviceToken(strDeviceToken)
                //                           .WithAlert(strMessage)
                //                           .WithBadge(7)
                //                           .WithSound("default"));

                //push.StopAllServices();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, INotification notification)
        {
            Console.WriteLine("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        //static void ChannelException(object sender, IPushChannel channel, Exception exception)
        //{
        //    Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        //}

        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        //static void ChannelCreated(object sender, IPushChannel pushChannel)
        //{
        //    Console.WriteLine("Channel Created for: " + sender);
        //}
    }
}
