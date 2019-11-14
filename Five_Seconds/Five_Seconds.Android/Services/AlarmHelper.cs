using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
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
        }

        public static void SetRepeatAlarm(Alarm alarm)
        {
            long diffMillis = 0;

            if (DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                diffMillis = CalculateTimeDiff(alarm);
            }

            if (diffMillis != 0)
            {
                SetAlarmByManager(alarm, diffMillis);
            }
        }

        public static void SetLaterAlarm(Alarm alarm)
        {
            var diffTimeSpan = alarm.LaterAlarmTime.Subtract(DateTime.Now);
            SetAlarmByManager(alarm, (long)diffTimeSpan.TotalMilliseconds);
        }

        private static long CalculateTimeDiff(Alarm alarm)
        {
            var dateTimeNow = DateTime.Now;

            var nextAlarmDateTime = alarm.NextAlarmTime;

            var diffTimeSpan = nextAlarmDateTime.Subtract(dateTimeNow);

            return (long)diffTimeSpan.TotalMilliseconds;
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
            _alarmIntent.PutExtra("id", alarm.Id);
            _alarmIntent.PutExtra("name", alarm.Name);
            _alarmIntent.PutExtra("IsAlarmOn", alarm.IsAlarmOn);
            _alarmIntent.PutExtra("IsVibrateOn", alarm.IsVibrateOn);
            _alarmIntent.PutExtra("IsVoiceRecognition", alarm.IsVoiceRecognition);
            _alarmIntent.PutExtra("IsNotDelayAlarm", alarm.IsNotDelayAlarm);
            _alarmIntent.PutExtra("IsFiveCount", alarm.IsFiveCount);
            _alarmIntent.PutExtra("IsRepeating", DaysOfWeek.GetHasADayBeenSelected(alarm.Days));
            _alarmIntent.PutExtra("toneName", alarm.Tone);
            _alarmIntent.PutExtra("alarmVolume", alarm.Volume);

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

        public static void RefreshAlarmByManager(List<Alarm> alarms)
        {
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            Intent alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));

            var maxId = Preferences.Get("MaxAlarmId", 3);

            for (int id = 1; id <= maxId; id++)
            {
                PendingIntent pendingAlarmIntent = PendingIntent.GetBroadcast(Application.Context, id, alarmIntent, PendingIntentFlags.UpdateCurrent);

                alarmManager.Cancel(pendingAlarmIntent);
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

        public static void RefreshAlarmByManager100(List<Alarm> alarms)
        {
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            Intent alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));

            for (int id = 1; id <= 100; id++)
            {
                PendingIntent pendingAlarmIntent = PendingIntent.GetBroadcast(Application.Context, id, alarmIntent, PendingIntentFlags.UpdateCurrent);

                alarmManager.Cancel(pendingAlarmIntent);
            }

            foreach (var alarm in alarms)
            {
                if (alarm.IsActive)
                {
                    SetAlarmAtFirst(alarm);
                }
            }
        }
    }
}