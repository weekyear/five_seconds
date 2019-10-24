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
using Five_Seconds.Services;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    public class NotificationReceiver : BroadcastReceiver
    {
        private IAlarmService alarmService;
        public override void OnReceive(Context context, Intent intent)
        {
            var bundle = intent.Extras;

            var id = bundle.GetInt("id", -100000);


            switch (intent.Action)
            {
                case "알람 해제":
                    GetAlarmService();
                    Alarm.IsInitFinished = false;
                    // 서비스에서 TurnOffAlarm();
                    var alarm = alarmService.GetAlarm(id);
                    alarm.IsActive = false;
                    alarmService.TurnOffAlarm(alarm);
                    Alarm.IsInitFinished = true;
                    break;
                case "지금 울림":
                    OpenAlarmActivity(context, bundle);
                    break;
            }

            var extras = intent.Extras;
            if (extras != null && !extras.IsEmpty)
            {
                NotificationManager manager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (id != -100000)
                {
                    manager.Cancel(id);
                }
            }
        }

        private void GetAlarmService()
        {
            if (App.Service == null)
            {
                alarmService = AlarmController.CreateServiceWithoutCore();
            }
            else
            {
                alarmService = App.Service;
            }
        }

        private void OpenAlarmActivity(Context context, Bundle bundle)
        {
            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtras(bundle);

            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
        }
    }
}