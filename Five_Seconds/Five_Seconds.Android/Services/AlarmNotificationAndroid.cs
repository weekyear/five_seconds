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
using Five_Seconds.Droid.BroadcastReceivers;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AlarmNotificationAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class AlarmNotificationAndroid : IAlarmNotification
    {
        private static readonly string NOTIFICATION_CHANNEL_ID = "com.beside.five_seconds";
        private static readonly string channelName = "알람";

        public static NotificationManager SetNotificationManager()
        {
            var manager = Application.Context.GetSystemService("notification") as NotificationManager;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName, NotificationImportance.Low);


                manager?.CreateNotificationChannel(chan);
            }

            return manager;
        }

        public static Notification GetNextAlarmNotification(Context context)
        {
            string nextNameString;
            string nextTimeString;

            var alarm = App.Service.GetNextAlarm();

            if (alarm != null)
            {
                nextNameString = alarm.Name;
                nextTimeString = alarm.NextAlarmTime.ToString();
            }
            else
            {
                nextNameString = "다음 알람이 없습니다.";
                nextTimeString = string.Empty;
            }
            

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
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

        public void CancelAllNotification()
        {
            var notificationManager = Application.Context.GetSystemService("notification") as NotificationManager;
            notificationManager.CancelAll();
        }

        public static void NotifyLaterAlarm(Alarm alarm, Intent intent)
        {
            SetNotificationManager();

            var manager = Application.Context.GetSystemService("notification") as NotificationManager;

            string nameString;
            string timeString;

            if (alarm != null)
            {
                nameString = alarm.Name;
                timeString = $"{alarm.LaterAlarmTime.ToShortTimeString()}에 울립니다";
            }
            else
            {
                nameString = "다음 알람이 없습니다.";
                timeString = string.Empty;
            }

            var bundle = intent.Extras;

            var actionIntent1 = CreateActionIntent(bundle, "알람 해제");
            var pIntent1 = PendingIntent.GetBroadcast(Application.Context, alarm.Id, actionIntent1, PendingIntentFlags.OneShot);

            var actionIntent2 = CreateActionIntent(bundle, "지금 울림");
            var pIntent2 = PendingIntent.GetBroadcast(Application.Context, alarm.Id, actionIntent2, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(Application.Context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(nameString)
                    .SetContentText(timeString)
                    .SetPriority((int)NotificationImportance.Low)
                    .SetVisibility(NotificationCompat.VisibilitySecret)
                    .SetContentIntent(OpenAppIntent())
                    .AddAction(0, "알람 해제", pIntent1)
                    .AddAction(0, "지금 울림", pIntent2)
                    .SetAutoCancel(false)
                    .Build();

            //var intentFilter = new IntentFilter();
            //intentFilter.AddAction("알람 해제");
            //intentFilter.AddAction("지금 울림");

            //var notificationReceiver = new NotificationReceiver();

            //Application.Context.RegisterReceiver(notificationReceiver, intentFilter);

            manager.Notify(alarm.Id, notification);
        }

        private static Intent CreateActionIntent(Bundle bundle, string action)
        {
            var actionIntent = new Intent(Application.Context, typeof(NotificationReceiver));
            actionIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            actionIntent.PutExtras(bundle);
            actionIntent.SetAction(action);

            return actionIntent;
        }

        private static PendingIntent OpenAppIntent()
        {
            Intent notificationIntent = new Intent(Application.Context, typeof(MainActivity));

            notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            
            PendingIntent pendingIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, 0);

            return pendingIntent;
        }

        public static void CancelLaterNotification(Context context, int id)
        {
            NotificationManager manager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            if (id != -100000)
            {
                Console.WriteLine("CancelNotification_NotificationReceiver");
                manager.Cancel(id);
            }
        }
    }
}