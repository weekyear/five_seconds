using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Five_Seconds.Droid.BroadcastReceivers;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using SQLite;
using Xamarin.Essentials;

namespace Five_Seconds.Droid.Services
{
    public class AlarmHelper
    {
        public static void SetAlarmAtFirst(Alarm alarm)
        {
            var diffMillis = CalculateTimeDiff(alarm);

            SetAlarmByManager(alarm, diffMillis);

            if (!alarm.IsGoOffPreAlarm) SetNotifiyPreAlarm(alarm, diffMillis);
        }

        public static void SetRepeatAlarm(Alarm alarm)
        {
            long diffMillis = 0;

            if (DaysOfWeek.GetHasADayBeenSelected(alarm.Days)) diffMillis = CalculateTimeDiffForRepeatAlarm(alarm);

            if (diffMillis != 0) SetAlarmByManager(alarm, diffMillis);

            SetNotifiyPreAlarm(alarm, diffMillis);
        }

        private static void SetNotifiyPreAlarm(Alarm alarm, long diffMillis)
        {
            if (diffMillis < TimeSpan.FromMinutes(5).TotalMilliseconds)
            {
                NotificationAndroid.NotifyPreAlarm(alarm, SetAlarmIntent(alarm));
            }
            else
            {
                SetNotificationPreAlarmByManager(alarm, diffMillis);
            }
        }

        public static void SetLaterAlarm(Alarm alarm)
        {
            var diffTimeSpan = alarm.LaterAlarmTime.Subtract(DateTime.Now);
            SetAlarmByManager(alarm, (long)diffTimeSpan.TotalMilliseconds);
        }

        private static long CalculateTimeDiff(Alarm alarm)
        {
            var nextAlarmDateTime = alarm.NextAlarmTime;

            var diffTimeSpan = nextAlarmDateTime.Subtract(DateTime.Now);

            return (long)diffTimeSpan.TotalMilliseconds;
        }

        private static long CalculateTimeDiffForRepeatAlarm(Alarm alarm)
        {
            var nextAlarmDateTime = alarm.NextAlarmTimeExceptForPreAlarm;

            var diffTimeSpan = nextAlarmDateTime.Subtract(DateTime.Now);

            return (long)diffTimeSpan.TotalMilliseconds;
        }

        private static void SetNotificationPreAlarmByManager(Alarm alarm, long diffMillis)
        {
            var _alarmIntent = SetPreAlarmIntent(alarm);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, alarm.Id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            Intent showIntent = new Intent(Application.Context, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(Application.Context, alarm.Id, showIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager.AlarmClockInfo alarmClockInfo = new AlarmManager.AlarmClockInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + diffMillis - (long)TimeSpan.FromMinutes(5).TotalMilliseconds, showOperation);
            alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
        }

        private static Intent SetPreAlarmIntent(Alarm alarm)
        {
            var _alarmIntent = new Intent(Application.Context, typeof(PreAlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtras(GetAlarmBundle(alarm));
            return _alarmIntent;
        }

        public static void DeletePreAlarmByManager(int id)
        {
            var alarmIntent = new Intent(Application.Context, typeof(PreAlarmReceiver));
            alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            alarmIntent.PutExtra("id", id);

            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            var toDeletePendingIntent = PendingIntent.GetBroadcast(Application.Context, id, alarmIntent, PendingIntentFlags.UpdateCurrent);
            alarmManager.Cancel(toDeletePendingIntent);
        }

        public static void SetAlarmByManager(Alarm alarm, long diffMillis)
        {
            var _alarmIntent = SetAlarmIntent(alarm);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, alarm.Id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            Intent showIntent = new Intent(Application.Context, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(Application.Context, alarm.Id, showIntent, PendingIntentFlags.UpdateCurrent);

            AlarmManager.AlarmClockInfo alarmClockInfo = new AlarmManager.AlarmClockInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + diffMillis, showOperation);
            alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
        }

        private static Intent SetAlarmIntent(Alarm alarm)
        {
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtras(GetAlarmBundle(alarm));
            return _alarmIntent;
        }

        public static void SetAllAlarmWhenRestart()
        {
            var service = CreateServiceWithoutCore();

            Alarm.IsInitFinished = false;
            var alarms = service.GetAllAlarms();
            Alarm.IsInitFinished = true;

            RefreshAlarmByManager(alarms);
        }

        public static AlarmService CreateServiceWithoutCore()
        {
            var deviceStorage = new DeviceStorageAndroid();
            var sqliteConnection = new SQLiteConnection(deviceStorage.GetFilePath("AlarmsSQLite.db3"));
            var itemDatabase = new ItemDatabaseGeneric(sqliteConnection);
            var alarmsRepo = new AlarmsRepository(itemDatabase);
            var service = new AlarmService(alarmsRepo);

            return service;
        }

        public static void DeleteAlarmByManager(int id)
        {
            var alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            alarmIntent.PutExtra("id", id);

            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            var toDeletePendingIntent = PendingIntent.GetBroadcast(Application.Context, id, alarmIntent, PendingIntentFlags.UpdateCurrent);
            alarmManager.Cancel(toDeletePendingIntent);
        }

        public static void RefreshAlarmByManager(IEnumerable<Alarm> alarms)
        {
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            using (var alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver)))
            {
                var maxId = Preferences.Get("MaxAlarmId", 3);

                for (int id = 1; id <= maxId; id++)
                {
                    PendingIntent pendingAlarmIntent = PendingIntent.GetBroadcast(Application.Context, id, alarmIntent, PendingIntentFlags.UpdateCurrent);

                    alarmManager.Cancel(pendingAlarmIntent);
                }
            }

            foreach (var alarm in alarms)
            {
                if (alarm.IsLaterAlarm)
                {
                    SetLaterAlarm(alarm);
                }
                else if (alarm.IsActive)
                {
                    SetAlarmAtFirst(alarm);
                }
            }
        }

        public static void RefreshAlarmByManager100(IEnumerable<Alarm> alarms)
        {
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            using (Intent alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver)))
            {
                for (int id = 1; id <= 100; id++)
                {
                    PendingIntent pendingAlarmIntent = PendingIntent.GetBroadcast(Application.Context, id, alarmIntent, PendingIntentFlags.UpdateCurrent);

                    alarmManager.Cancel(pendingAlarmIntent);
                }
            }

            foreach (var alarm in alarms)
            {
                if (alarm.IsActive)
                {
                    SetAlarmAtFirst(alarm);
                }
            }
        }

        public static void SetComebackNotification(bool isAlarmComeback)
        {
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            PendingIntent pendingIntent;
            TimeSpan diffTimeSpan;
            int id;

            if (isAlarmComeback)
            {
                id = -99;
                diffTimeSpan = new TimeSpan(7, 0, 0, 0);
            }
            else
            {
                id = -98;
                diffTimeSpan = new TimeSpan(15, 0, 0, 0);
            }

            var _alarmIntent = new Intent(Application.Context, typeof(ComebackReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", id);

            pendingIntent = PendingIntent.GetBroadcast(Application.Context, id, _alarmIntent, PendingIntentFlags.UpdateCurrent);

            var diffMillis = (long)diffTimeSpan.TotalMilliseconds;

            Intent showIntent = new Intent(Application.Context, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(Application.Context, id, showIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager.AlarmClockInfo alarmClockInfo = new AlarmManager.AlarmClockInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + diffMillis, showOperation);
            alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
        }

        private static Bundle GetAlarmBundle(Alarm alarm)
        {
            var bundle = new Bundle();
            bundle.PutInt("id", alarm.Id);
            bundle.PutString("name", alarm.Name);
            bundle.PutBoolean("IsAlarmOn", alarm.IsAlarmOn);
            bundle.PutBoolean("IsVibrateOn", alarm.IsVibrateOn);
            bundle.PutBoolean("IsVoiceRecognition", alarm.IsVoiceRecognition);
            bundle.PutBoolean("IsNotDelayAlarm", alarm.IsNotDelayAlarm);
            bundle.PutBoolean("IsFiveCount", alarm.IsFiveCount);
            bundle.PutBoolean("IsGoOffPreAlarm", alarm.IsGoOffPreAlarm);
            bundle.PutBoolean("IsTurnOffPreAlarm", alarm.IsTurnOffPreAlarm);
            bundle.PutBoolean("HasWakeUpText", alarm.HasWakeUpText);
            bundle.PutString("WakeUpText", alarm.WakeUpText);
            bundle.PutBoolean("IsLinkOtherApp", alarm.IsLinkOtherApp);
            bundle.PutString("PackageName", alarm.PackageName);
            bundle.PutBoolean("IsRepeating", DaysOfWeek.GetHasADayBeenSelected(alarm.Days));
            bundle.PutString("toneName", alarm.Tone);
            bundle.PutInt("alarmVolume", alarm.Volume);

            return bundle;
        }
    }
}