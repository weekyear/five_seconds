using System;
using Android.App;
using Android.Content;
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

            return Java.Lang.JavaSystem.CurrentTimeMillis() + (long)diffTimeSpan.TotalMilliseconds;
        }

        private static void ShowNextAlarmToast(DateTime dateTime)
        {
            var diffString = CreateDateString.CreateTimeRemainingString(dateTime);

            var toastService = new ToastServiceAndroid();

            toastService.Show(diffString);
        }

        private static void SetAlarmByManager(Alarm alarm, long diffMillis)
        {
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", alarm.Id);
            _alarmIntent.PutExtra("name", alarm.Name);
            _alarmIntent.PutExtra("isAlarmOn", alarm.IsAlarmOn);
            _alarmIntent.PutExtra("isVibrateOn", alarm.IsVibrateOn);
            _alarmIntent.PutExtra("isCountOn", alarm.IsCountSoundOn);
            _alarmIntent.PutExtra("isRepeating", DaysOfWeek.GetHasADayBeenSelected(alarm.Days));
            _alarmIntent.PutExtra("toneName", alarm.Tone);
            _alarmIntent.PutExtra("alarmVolume", alarm.Volume);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, alarm.Id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            Intent showIntent = new Intent(Application.Context, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(Application.Context, 0, showIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager.AlarmClockInfo alarmClockInfo = new AlarmManager.AlarmClockInfo(diffMillis, showOperation);
            alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
        }

        public static void SetAllAlarmWhenRestart()
        {
            var deviceStorage = new DeviceStorageAndroid();
            var itemDatabase = new ItemDatabaseGeneric(new SQLiteConnection(deviceStorage.GetFilePath("AlarmsSQLite.db3")));
            var alarmsRepo = new AlarmsRepository(itemDatabase);
            var service = new AlarmService(alarmsRepo);
            var alarms = service.GetAllAlarms();

            foreach (var alarm in alarms)
            {
                SetFirstAlarm(alarm);
            }
        }
    }
}