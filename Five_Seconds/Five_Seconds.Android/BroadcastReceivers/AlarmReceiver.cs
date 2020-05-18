using Android.App;
using Android.Content;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using SQLite;
using System;
using System.Collections.Generic;

namespace Five_Seconds.Droid.BroadcastReceivers
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        private int id;
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_AlarmReceiver");
            var bundle = intent.Extras;
            id = (int)bundle.Get("id");
            var IsTurnOffPreAlarm = (bool)bundle.Get("IsTurnOffPreAlarm");
            Console.WriteLine($"Id_AlarmReceiver : {id}");

            NotificationAndroid.CancelLaterNotification(context, id);

            if (!IsTurnOffPreAlarm)
            {
                var disIntent = new Intent(context, typeof(AlarmActivity));
                disIntent.PutExtras(bundle);

                disIntent.SetFlags(ActivityFlags.NewTask);
                context.StartActivity(disIntent);
            }
            else
            {
                var alarm = GetAlarmById();
                alarm.IsGoOffPreAlarm = false;
                App.AlarmService.SaveAlarm(alarm);
            }
        }
        private Alarm GetAlarmById()
        {
            var alarmService = App.AlarmService;
            Alarm.IsInitFinished = false;
            var alarm = alarmService.GetAlarm(id);
            Alarm.IsInitFinished = true;

            return alarm;
        }
    }
}
