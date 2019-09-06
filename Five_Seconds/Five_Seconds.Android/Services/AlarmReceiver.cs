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
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OPEN THE THING");
            var id = intent.GetIntExtra("id", 0);

            var mission = App.MissionsRepo.GetMission(id);
            mission.Alarm = App.MissionsRepo.GetAlarm(mission.AlarmId);
            mission.Alarm.Days = App.MissionsRepo.GetDaysOfWeek(mission.Alarm.DaysId);
            var diffMillis = CalculateTimeDiff(mission.Alarm);

            if (mission.IsActive)
            {
                SetAlarmByManager(id, diffMillis);
                //StartAlarmActivity(context, id);
            }

            if (diffMillis == 0)
            {
                mission.IsActive = false;
                App.Service.SaveMissionAtLocal(mission);
            }

            SetNewNotification();
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
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

            alarmManager.SetExact(AlarmType.RtcWakeup, diffMillis, pendingIntent);
            //alarmManager.SetExact(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + 10000, pendingIntent);
        }
        private void SetNewNotification()
        {
            var manager = Application.Context.GetSystemService("notification") as NotificationManager;

            var notification = AlarmNotification.GetNextAlarmNotification(Application.Context);

            manager.Notify(2, notification);
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