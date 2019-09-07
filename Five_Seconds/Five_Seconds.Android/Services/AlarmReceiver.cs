using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public static Mission AlarmMissionNow;
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OPEN THE THING");
            var id = intent.GetIntExtra("id", 0);

            AlarmMissionNow = App.MissionsRepo.GetMission(id);
            AlarmMissionNow.Alarm = App.MissionsRepo.GetAlarm(AlarmMissionNow.AlarmId);
            AlarmMissionNow.Alarm.Days = App.MissionsRepo.GetDaysOfWeek(AlarmMissionNow.Alarm.DaysId);
            var diffMillis = CalculateTimeDiff(AlarmMissionNow.Alarm);

            if (AlarmMissionNow.IsActive)
            {
                SetAlarmByManager(id, diffMillis);
                StartAlarmActivity(context, id);
            }
        }

        private long CalculateTimeDiff(Alarm alarm)
        {
            var dateTimeNow = DateTime.Now;

            var nextAlarmDateTime = alarm.NextAlarmTime;

            var diffTimeSpan = nextAlarmDateTime.Subtract(dateTimeNow);

            if (diffTimeSpan.TotalMilliseconds <= 0)
            {
                return 0;
            }
            else
            {
                ShowNextAlarmToast(diffTimeSpan);

                return Java.Lang.JavaSystem.CurrentTimeMillis() + (long)diffTimeSpan.TotalMilliseconds;
            }
        }

        private void ShowNextAlarmToast(TimeSpan diff)
        {
            var diffString = CreateTimeRemainingString(diff);

            DependencyService.Get<ToastService>().Show(diffString);
        }

        private string CreateTimeRemainingString(TimeSpan diff)
        {
            if (diff.Days > 0)
            {
                return $"{diff.Days + 1}일 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Hours > 0)
            {
                return $"{diff.Hours}시간 {diff.Minutes}분 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Minutes > 0)
            {
                return $"{diff.Minutes}분 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Seconds > 0)
            {
                return $"{diff.Seconds}초 후에 5초의 법칙을 실행합니다!";
            }
            else
            {
                return "이미 지난 시간입니다.";
            }
        }

        private void SetAlarmByManager(int id, long diffMillis)
        {
            if (diffMillis == 0)
            {
                AlarmMissionNow.IsActive = false;
                return;
            }
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

            alarmManager.SetExact(AlarmType.RtcWakeup, diffMillis, pendingIntent);
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