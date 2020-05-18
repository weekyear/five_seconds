using System.Collections.Generic;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Application = Android.App.Application;

[assembly: Dependency(typeof(AlarmSetterAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class AlarmSetterAndroid : IAlarmSetter
    {
        public AlarmSetterAndroid() { }

        public void SetAlarm(Alarm alarm)
        {
            NotificationAndroid.CancelLaterNotification(Application.Context, alarm.Id);
            AlarmHelper.SetAlarmAtFirst(alarm);
        }

        public void DeleteAlarm(int id)
        {
            AlarmHelper.DeleteAlarmByManager(id);
            AlarmHelper.DeletePreAlarmByManager(id);
            NotificationAndroid.CancelLaterNotification(Application.Context, id);
        }

        public void DeleteAllAlarms(IEnumerable<Alarm> alarms)
        {
            alarms.ForEach((alarm) => DeleteAlarm(alarm.Id));
        }
    }
}