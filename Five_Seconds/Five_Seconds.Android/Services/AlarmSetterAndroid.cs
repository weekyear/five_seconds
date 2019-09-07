using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.Views;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(AlarmSetterAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class AlarmSetterAndroid : IAlarmSetter
    {
        public static string AlarmTag = "Al4rm";
        IAlarmToneRepository _alarmRepo = App.AlarmToneRepo;

        public AlarmSetterAndroid()
        {

        }

        public void SetAlarm(Mission mission)
        {
            var alarm = mission.Alarm;
            var diffMillis = CalculateTimeDiff(alarm);

            var serviceIntent = new Intent(Application.Context, typeof(AlarmService));
            serviceIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            serviceIntent.PutExtra("id", mission.Id);
            serviceIntent.PutExtra("diffMillis", diffMillis);
            serviceIntent.SetAction("ActionStartService");

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                Application.Context.StartForegroundService(serviceIntent);
            }
            else
            {
                Application.Context.StartService(serviceIntent);
            }
        }
        private long CalculateTimeDiff(Alarm alarm)
        {
            var dateTimeNow = DateTime.Now;

            var nextAlarmDateTime = alarm.NextAlarmTime;

            var diffTimeSpan = nextAlarmDateTime.Subtract(dateTimeNow);

            ShowNextAlarmToast(diffTimeSpan);

            return Java.Lang.JavaSystem.CurrentTimeMillis() + (long)diffTimeSpan.TotalMilliseconds;
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


        public void DeleteAlarm(int id)
        {
            var alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            alarmIntent.PutExtra("id", id);

            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            var toDeletePendingIntent = PendingIntent.GetBroadcast(Application.Context, id, alarmIntent, PendingIntentFlags.UpdateCurrent);
            alarmManager.Cancel(toDeletePendingIntent);
        }

        public void DeleteAllAlarms(List<Mission> missions)
        {
            foreach (Mission mission in missions)
            {
                DeleteAlarm(mission.Id);
            }
        }
    }
}