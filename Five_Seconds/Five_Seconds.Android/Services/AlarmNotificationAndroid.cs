using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly string channelName = "Alarm";

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

        public static void NotifyFailedAlarm(Alarm alarm)
        {
            SetNotificationManager();

            var context = Application.Context;

            var manager = context.GetSystemService("notification") as NotificationManager;

            string title;
            string message;

            if (alarm != null)
            {
                title = context.GetString(Resource.String.Failure);
                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        message = $"{alarm.LaterAlarmTime.ToShortTimeString()}에 {alarm.Name}를 실패하였습니다.";
                        break;
                    case "en-US":
                        message = $"You failed {alarm.Name} at {alarm.LaterAlarmTime.ToShortTimeString()}";
                        break;
                    default:
                        message = $"You failed {alarm.Name} at {alarm.LaterAlarmTime.ToShortTimeString()}";
                        break;
                }
            }
            else
            {
                title = context.GetString(Resource.String.Failure);
                message = string.Empty;
            }

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(false)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetPriority((int)NotificationImportance.Low)
                    .SetVisibility(NotificationCompat.VisibilitySecret)
                    .SetContentIntent(OpenAppIntent())
                    .SetAutoCancel(true)
                    .Build();

            manager.Notify(alarm.Id, notification);
        }

        public static void NotifyLaterAlarm(Alarm alarm, Intent intent)
        {
            SetNotificationManager();

            var context = Application.Context;

            var manager = context.GetSystemService("notification") as NotificationManager;

            string nameString;
            string timeString;

            if (alarm != null)
            {
                nameString = alarm.Name;

                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        timeString = $"{alarm.LaterAlarmTime.ToShortTimeString()}에 울립니다";
                        break;
                    case "en-US":
                        timeString = $"Alarm will ring at {alarm.LaterAlarmTime.ToShortTimeString()}";
                        break;
                    default:
                        timeString = $"Alarm will ring at {alarm.LaterAlarmTime.ToShortTimeString()}";
                        break;
                }
            }
            else
            {
                nameString = "다음 알람이 없습니다.";
                timeString = string.Empty;
            }

            var bundle = intent.Extras;

            var actionIntent1 = CreateActionIntent(bundle, context.GetString(Resource.String.AlarmOff));
            var pIntent1 = PendingIntent.GetBroadcast(context, alarm.Id, actionIntent1, PendingIntentFlags.OneShot);

            var actionIntent2 = CreateActionIntent(bundle, context.GetString(Resource.String.GoOffNow));
            var pIntent2 = PendingIntent.GetBroadcast(context, alarm.Id, actionIntent2, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(nameString)
                    .SetContentText(timeString)
                    .SetPriority((int)NotificationImportance.Low)
                    .SetVisibility(NotificationCompat.VisibilitySecret)
                    .SetContentIntent(OpenAppIntent())
                    .AddAction(0, context.GetString(Resource.String.AlarmOff), pIntent1)
                    .AddAction(0, context.GetString(Resource.String.GoOffNow), pIntent2)
                    .SetAutoCancel(false)
                    .Build();

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
                manager.Cancel(id);
            }
        }
    }
}