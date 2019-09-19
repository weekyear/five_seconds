using Android.Content;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using SQLite;
using System;
using System.Collections.Generic;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        private int id;
        private string name;
        private bool isAlarmOn;
        private bool isVibrateOn;
        private bool isCountOn;
        private bool isRepeating;
        private string toneName;
        private int alarmVolume;
        public override void OnReceive(Context context, Intent intent)
        {
            id = intent.GetIntExtra("id", 0);
            name = intent.GetStringExtra("name");
            isAlarmOn = intent.GetBooleanExtra("isAlarmOn", false);
            isVibrateOn = intent.GetBooleanExtra("isVibrateOn", false);
            isCountOn = intent.GetBooleanExtra("isCountOn", false);
            isRepeating = intent.GetBooleanExtra("isRepeating", false);
            toneName = intent.GetStringExtra("toneName");
            alarmVolume = intent.GetIntExtra("alarmVolume", 0);

            StartAlarmActivity(context);
        }

        private void StartAlarmActivity(Context context)
        {
            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtra("id", id);
            disIntent.PutExtra("name", name);
            disIntent.PutExtra("isAlarmOn", isAlarmOn);
            disIntent.PutExtra("isVibrateOn", isVibrateOn);
            disIntent.PutExtra("isCountOn", isCountOn);
            disIntent.PutExtra("isRepeating", isRepeating);
            disIntent.PutExtra("toneName", toneName);
            disIntent.PutExtra("alarmVolume", alarmVolume);

            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
        }
    }
}