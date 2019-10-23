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
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", alarm.Id);
            _alarmIntent.PutExtra("name", alarm.Name);
            _alarmIntent.PutExtra("IsAlarmOn", alarm.IsAlarmOn);
            _alarmIntent.PutExtra("IsVibrateOn", alarm.IsVibrateOn);
            _alarmIntent.PutExtra("IsCountSoundOn", alarm.IsCountSoundOn);
            _alarmIntent.PutExtra("IsCountOn", alarm.IsCountOn);
            _alarmIntent.PutExtra("IsRepeating", DaysOfWeek.GetHasADayBeenSelected(alarm.Days));
            _alarmIntent.PutExtra("toneName", alarm.Tone);
            _alarmIntent.PutExtra("alarmVolume", alarm.Volume);
            _alarmIntent.PutExtra("IsLaterAlarm", false);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, alarm.Id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            Intent showIntent = new Intent(Application.Context, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(Application.Context, 0, showIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager.AlarmClockInfo alarmClockInfo = new AlarmManager.AlarmClockInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + diffMillis, showOperation);
            alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
        }

        public static void SetAllAlarmWhenRestart()
        {
            try
            {
                Console.WriteLine("SetAllAlarmWhenRestart_AlarmController");
                var deviceStorage = new DeviceStorageAndroid();
                Console.WriteLine("DeviceStorageAndroid_AlarmController");
                var sqliteConnection = new SQLiteConnection(deviceStorage.GetFilePath("AlarmsSQLite.db3"));
                Console.WriteLine("SQLiteConnection_AlarmController");
                var itemDatabase = new ItemDatabaseGeneric(sqliteConnection);
                Console.WriteLine("ItemDatabaseGeneric_AlarmController");
                var alarmsRepo = new AlarmsRepository(itemDatabase);
                Console.WriteLine("AlarmsRepository_AlarmController");
                var service = new AlarmService(alarmsRepo);
                Console.WriteLine("AlarmService_AlarmController");
                var alarms = service.GetAllAlarms();
                Console.WriteLine("GetAllAlarms_AlarmController");
                Console.WriteLine($"alarms Count : {alarms.Count}");

                foreach (var alarm in alarms)
                {
                    Console.WriteLine($"alarms Name : {alarm.Name}");
                    SetFirstAlarm(alarm);
                }
                Console.WriteLine("SetAllAlarmWhenRestart_AlarmController");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Message :: {e.Message}");
                Console.WriteLine($"Error InnerException :: {e.InnerException}");
                Console.WriteLine($"Error StackTrace :: {e.StackTrace}");
                Console.WriteLine($"Error Source :: {e.Source}");
            }
        }
    }
}