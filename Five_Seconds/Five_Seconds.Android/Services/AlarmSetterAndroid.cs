using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(AlarmSetterAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class AlarmSetterAndroid : IAlarmSetter
    {
        public static string AlarmTag = "Al4rm";
        IAlarmToneRepository _alarmRepo = App.AlarmRepo;

        public AlarmSetterAndroid()
        {

        }

        public void SetAlarm(Mission mission)
        {
            var dateNow = DateTime.Now.ToLocalTime().TimeOfDay;
            var alarm = mission.Alarm;
            var difference = alarm.Time.Subtract(dateNow);
            // 마이너스값일 때 12시간 추가해줘야 함
            var differenceAsMillis = difference.TotalMilliseconds;

            alarm.Time.Add(new TimeSpan(1, 0, 0));

            var receiverIntent = new Intent(Application.Context, typeof(AlarmService));
            receiverIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            receiverIntent.PutExtra("id", mission.Id);
            receiverIntent.PutExtra("diffAsMillis", differenceAsMillis);
            receiverIntent.SetAction("ActionStartService");

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                Application.Context.StartForegroundService(receiverIntent);
            }
            else
            {
                Application.Context.StartService(receiverIntent);
            }
        }

        public void SetRepeatingAlarm(Alarm alarm)
        {
        }

        public void DeleteAlarm(int id)
        {
            var alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            alarmIntent.PutExtra("id", id);

            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            var toDeletePendingIntent = PendingIntent.GetBroadcast(Application.Context, id, alarmIntent, PendingIntentFlags.CancelCurrent);
            alarmManager.Cancel(toDeletePendingIntent);
        }

        string GetTimeDifferenceAsString(DateTime alarmTime)
        {
            var timeFromNow = GetTimeDifferenceInHours(alarmTime);

            StringBuilder sb = new StringBuilder();

            if (timeFromNow.Days > 0)
            {
                Log.Debug(AlarmTag, "total days: " + timeFromNow.Days);
                sb.Append(timeFromNow.Days);
                sb.Append(" days");
            }

            if (timeFromNow.Hours > 0)
            {
                Log.Debug(AlarmTag, "total hours: " + timeFromNow.Hours);
                if (sb.Length > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(timeFromNow.Hours);
                sb.Append(" hours");
            }

            if (timeFromNow.Minutes > 0)
            {
                Log.Debug(AlarmTag, "total minutes: " + timeFromNow.Minutes);
                if (sb.Length > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(timeFromNow.Minutes);
                sb.Append(" minutes");
            }

            if (timeFromNow.Seconds > 0)
            {
                Log.Debug(AlarmTag, "total seconds: " + timeFromNow.Seconds);
                if (sb.Length > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(timeFromNow.Seconds);
                sb.Append(" seconds");
            }

            return sb.ToString();
        }

        TimeSpan GetTimeDifferenceInHours(DateTime alarmTime)
        {
            var now = DateTime.Now.TimeOfDay;
            var alarm = alarmTime.TimeOfDay;
            var diff = alarm.Subtract(now);

            return diff;
        }

        public void DeleteAllAlarms(List<Mission> missions)
        {
            foreach (Mission mission in missions)
            {
                DeleteAlarm(mission.Id);
            }
        }
    }

    [Service]
    public class AlarmService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                StartMyOwnForeground();
            else
                StartForeground(1, new Notification());

            
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            SetNotification(intent);

            return StartCommandResult.Sticky;
        }

        private void StartMyOwnForeground()
        {
            var NOTIFICATION_CHANNEL_ID = "com.example.simpleapp";
            var channelName = "My Background Service";
            var chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName, NotificationImportance.Low);
            var manager = (NotificationManager)GetSystemService(Context.NotificationService);

            manager?.CreateNotificationChannel(chan);

            var notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_plus)
                    .SetContentTitle("App is running in background")
                    .SetPriority((int)NotificationImportance.Min)
                    .SetCategory(Notification.CategoryService)
                    .SetChannelId(NOTIFICATION_CHANNEL_ID)
                    .Build();

            StartForeground(2, notification);

            //manager.Notify(2, notification);
            //manager.Cancel(2);
        }

        private void SetNotification(Intent intent)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OPEN THE THING");
            var id = intent.GetIntExtra("id", 0);
            var diffAsMillis = intent.GetDoubleExtra("diffAsMillis", 0);

            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

            alarmManager.SetExact(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)diffAsMillis, pendingIntent);
            //context.StartForegroundService(_alarmIntent);
            Log.Debug(AlarmSetterAndroid.AlarmTag, "START ACTIVITY");
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }

    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OPEN THE THING");
            var id = intent.GetIntExtra("id", 0);

            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtra("id", id);
            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
            Log.Debug(AlarmSetterAndroid.AlarmTag, "START ACTIVITY");
        }
    }

    
}