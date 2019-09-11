using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.Models;

namespace Five_Seconds.Droid.Services
{
    public class AlarmController
    {
        public static Mission AlarmMissionNow;
        public static void SetNextAlarm(int id)
        {
            Console.WriteLine(App.MissionsRepo);
            if (App.MissionsRepo != null)
            {
                AlarmMissionNow = App.MissionsRepo.GetMission(id);
                Console.WriteLine(AlarmMissionNow.Name);
                AlarmMissionNow.Alarm = App.MissionsRepo.GetAlarm(AlarmMissionNow.AlarmId);
                Console.WriteLine(AlarmMissionNow.Alarm.Time);
                AlarmMissionNow.Alarm.Days = App.MissionsRepo.GetDaysOfWeek(AlarmMissionNow.Alarm.DaysId);
                Console.WriteLine(AlarmMissionNow.Alarm.DaysId);
            }
            else
            {
                Console.WriteLine("App.MissionsRepo is null");
                return;
            }

            var diffMillis = CalculateTimeDiff(AlarmMissionNow.Alarm);

            if (AlarmMissionNow.IsActive)
            {
                SetAlarmByManager(id, diffMillis);
            }
        }

        private static long CalculateTimeDiff(Alarm alarm)
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

        private static void ShowNextAlarmToast(TimeSpan diff)
        {
            var diffString = CreateTimeRemainingString(diff);

            var toastService = new ToastServiceAndroid();

            toastService.Show(diffString);
        }

        private static string CreateTimeRemainingString(TimeSpan diff)
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

        private static void SetAlarmByManager(int id, long diffMillis)
        {
            Console.WriteLine("SetAlarmByManager_AlarmController");
            if (diffMillis == 0)
            {
                AlarmMissionNow.IsActive = false;
                return;
            }
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.NewTask | ActivityFlags.ExcludeFromRecents);
            _alarmIntent.PutExtra("id", id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService("alarm");

            alarmManager.SetExact(AlarmType.RtcWakeup, diffMillis, pendingIntent);
        }
    }
}