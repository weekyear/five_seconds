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
        public static void SetFirstAlarm(Mission mission)
        {
            var alarm = mission.Alarm;
            var diffMillis = CalculateTimeDiff(alarm);

            SetAlarmByManager(mission, diffMillis);
        }

        public static void SetNextAlarm(Mission mission)
        {
            long diffMillis = 0;

            if (DaysOfWeek.GetHasADayBeenSelected(mission.Alarm.Days))
            {
                diffMillis = CalculateTimeDiff(mission.Alarm);
            }

            if (diffMillis != 0)
            {
                SetAlarmByManager(mission, diffMillis);
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

        private static void SetAlarmByManager(Mission mission, long diffMillis)
        {
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", mission.Id);
            _alarmIntent.PutExtra("name", mission.Name);
            _alarmIntent.PutExtra("isAlarmOn", mission.Alarm.IsAlarmOn);
            _alarmIntent.PutExtra("isVibrateOn", mission.Alarm.IsVibrateOn);
            _alarmIntent.PutExtra("isCountOn", mission.Alarm.IsCountOn);
            _alarmIntent.PutExtra("isRepeating", DaysOfWeek.GetHasADayBeenSelected(mission.Alarm.Days));
            _alarmIntent.PutExtra("toneName", mission.Alarm.Tone);
            _alarmIntent.PutExtra("alarmVolume", mission.Alarm.Volume);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, mission.Id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            Intent showIntent = new Intent(Application.Context, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(Application.Context, 0, showIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager.AlarmClockInfo alarmClockInfo = new AlarmManager.AlarmClockInfo(diffMillis, showOperation);
            alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
            Console.WriteLine(mission.Name + "is Set Alarmed");
        }

        public static void SetAllAlarmWhenRestart()
        {
            Console.WriteLine("SetAllAlarmWhenRestart_AlarmController");
            var deviceStorage = new DeviceStorageAndroid();
            var itemDatabase = new ItemDatabaseGeneric(new SQLiteConnection(deviceStorage.GetFilePath("MissionsSQLite.db3")));
            var missionsRepo = new MissionsRepository(itemDatabase);
            var service = new MissionService(missionsRepo);
            var missions = service.GetAllMissions();

            foreach (var mission in missions)
            {
                SetFirstAlarm(mission);
            }
        }
    }
}