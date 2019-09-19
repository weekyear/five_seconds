using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AlarmNotificationAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class AlarmNotificationAndroid : IAlarmNotification
    {
        private static string NOTIFICATION_CHANNEL_ID = "com.beside.five_seconds";
        private static string channelName = "알람";

        public static NotificationManager SetNotificationManager()
        {
            var chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName, NotificationImportance.Low);

            var manager = Application.Context.GetSystemService("notification") as NotificationManager;

            manager?.CreateNotificationChannel(chan);

            return manager;
        }

        public static Notification GetNextAlarmNotification(Context context)
        {
            string nextNameString;
            string nextTimeString;

            var alarm = App.Service.GetNextAlarm();

            if (alarm != null)
            {
                var mission = App.MissionsRepo.GetMission(alarm.Id);

                nextNameString = mission.Name;
                nextTimeString = alarm.NextAlarmTime.ToString();
            }
            else
            {
                nextNameString = "다음 알람이 없습니다.";
                nextTimeString = string.Empty;
            }
            

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_icon_notificatioin)
                    .SetContentTitle(nextNameString)
                    .SetContentText(nextTimeString)
                    .SetPriority((int)NotificationImportance.Low)
                    .SetVisibility(NotificationCompat.VisibilitySecret)
                    .Build();

            return notification;
        }

        public void UpdateNotification()
        {
            var manager = Application.Context.GetSystemService("notification") as NotificationManager;

            var notification = GetNextAlarmNotification(Application.Context);

            manager.Notify(2, notification);
        }

        public void CancelNotification()
        {
            var notificationManager = Application.Context.GetSystemService("notification") as NotificationManager;
            notificationManager.CancelAll();
        }
    }
}