using System;
using Android.App;
using Android.Content;
using Android.Util;
using Five_Seconds.Models;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OPEN THE THING");
            var id = intent.GetIntExtra("id", 0);

            var mission = App.MissionsRepo.GetMission(id);
            mission.Alarm = App.MissionsRepo.GetAlarm(id);
            mission.Alarm.Days = App.MissionsRepo.GetDaysOfWeek(id);
            var diffMillis = CalculateNextAlarmMillis(mission.Alarm);

            if (mission.IsActive)
            {
                SetAlarmByManager(id, diffMillis);
                StartAlarmActivity(context, id);
            }

            if (diffMillis == 8)
            {
                mission.IsActive = false;
                App.Service.SaveMissionAtLocal(mission);
            }
        }

        private void SetAlarmByManager(int id, long diffMillis)
        {
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

            alarmManager.SetExact(AlarmType.RtcWakeup, diffMillis, pendingIntent);
        }

        private long CalculateNextAlarmMillis(Alarm alarm)
        {
            var allDays = alarm.Days.AllDays;

            int addingDays = 8;

            for (int i = 0; i < 7; i++)
            {
                if (allDays[i])
                {
                    var today = (int)DateTime.Now.DayOfWeek;
                    var diffDays = i - today >= 0 ? i - today : i - today + 7;
                    if (addingDays > diffDays)
                    {
                        addingDays = diffDays;
                    }
                }
            }

            if (addingDays == 8) return 8;

            var diffDaysMillis = new TimeSpan(addingDays, 0, 0, 0).TotalMilliseconds;

            var currentTimeMillis = alarm.TimeOffset.ToUnixTimeMilliseconds();

            return currentTimeMillis + (long)diffDaysMillis;
        }

        private void StartAlarmActivity(Context context, int id)
        {
            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtra("id", id);
            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
            Log.Debug(AlarmSetterAndroid.AlarmTag, "START ACTIVITY");
        }
    }
}