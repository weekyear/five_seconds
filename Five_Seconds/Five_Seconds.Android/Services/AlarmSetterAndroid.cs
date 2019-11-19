using System.Collections.Generic;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(AlarmSetterAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class AlarmSetterAndroid : IAlarmSetter
    {
        public AlarmSetterAndroid()
        {

        }

        public void SetAlarm(Alarm alarm)
        {
            AlarmHelper.SetAlarmAtFirst(alarm);
        }

        public void DeleteAlarm(int id)
        {
            AlarmHelper.DeleteAlarmByManager(id);
            NotificationAndroid.CancelLaterNotification(Application.Context, id);
        }

        public void DeleteAllAlarms(List<Alarm> alarms)
        {
            alarms.ForEach((alarm) => DeleteAlarm(alarm.Id));
        }
    }
}