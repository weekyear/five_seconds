using System;
using Android.App;
using Android.Content;
using Five_Seconds.Droid.BroadcastReceivers;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using SQLite;

namespace Five_Seconds.Droid.Services
{
    public class AlarmController
    {
        public static void SetFirstAlarm(Alarm alarm)
        {
            var diffMillis = CalculateTimeDiff(alarm);

            SetAlarmByManager(alarm, diffMillis);
        }

        public static void SetNextAlarm(Alarm alarm)
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

        private static long CalculateTimeDiff(Alarm alarm)
        {
            var dateTimeNow = DateTime.Now;

            var nextAlarmDateTime = alarm.NextAlarmTime;

            var diffTimeSpan = nextAlarmDateTime.Subtract(dateTimeNow);

            ShowNextAlarmToast(nextAlarmDateTime);

            return (long)diffTimeSpan.TotalMilliseconds;
        }

        private static void ShowNextAlarmToast(DateTime dateTime)
        {
            var diffString = CreateDateString.CreateTimeRemainingString(dateTime);

            var toastService = new ToastServiceAndroid();

            toastService.Show(diffString);
        }

        private static void SetAlarmByManager(Alarm alarm, long diffMillis)
        {
            var _alarmIntent = SetAlarmIntent(alarm);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, alarm.Id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            Intent showIntent = new Intent(Application.Context, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(Application.Context, alarm.Id, showIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager.AlarmClockInfo alarmClockInfo = new AlarmManager.AlarmClockInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + diffMillis, showOperation);
            alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
        }

        public static void SetLaterAlarmByManager(Alarm alarm, long diffMillis)
        {
            var _alarmIntent = SetAlarmIntent(alarm);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, -alarm.Id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            Intent showIntent = new Intent(Application.Context, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(Application.Context, -alarm.Id, showIntent, PendingIntentFlags.UpdateCurrent);
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
            _alarmIntent.PutExtra("IsRepeating", DaysOfWeek.GetHasADayBeenSelected(alarm.Days));
            _alarmIntent.PutExtra("toneName", alarm.Tone);
            _alarmIntent.PutExtra("alarmVolume", alarm.Volume);

            return _alarmIntent;
        }

        public static void SetAllAlarmWhenRestart()
        {
            var service = CreateServiceWithoutCore();
            var alarms = service.GetAllAlarms();

            foreach (var alarm in alarms)
            {
                SetFirstAlarm(alarm);
            }
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
    }
}