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
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;

namespace Five_Seconds.Droid.BroadcastReceivers
{
    [BroadcastReceiver]
    public class PreAlarmReceiver : BroadcastReceiver
    {
        private int id;
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_PreAlarmReceiver");
            var bundle = intent.Extras;
            id = (int)bundle.Get("id");
            Console.WriteLine($"Id_AlarmReceiver : {id}");

            var alarm = GetAlarmFromDB();

            NotificationAndroid.NotifyPreAlarm(GetAlarmFromDB(), intent);

            AlarmHelper.SetAlarmAtFirst(alarm);
        }
        private Alarm GetAlarmFromDB()
        {
            var alarmService = App.AlarmService;
            var alarmsRepo = alarmService.Repository;
            var alarm = alarmsRepo?.GetAlarm(id);
            alarm.Days = alarmsRepo?.GetDaysOfWeek(alarm.DaysId);

            return alarm;
        }
    }
}