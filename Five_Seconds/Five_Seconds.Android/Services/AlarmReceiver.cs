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
        private bool IsAlarmOn;
        private bool IsVibrateOn;
        private bool IsCountOn;
        private bool IsCountSoundOn;
        private bool IsRepeating;
        private string toneName;
        private int alarmVolume;
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_AlarmReceiver");
            id = intent.GetIntExtra("id", 0);
            name = intent.GetStringExtra("name");
            IsAlarmOn = intent.GetBooleanExtra("IsAlarmOn", false);
            IsVibrateOn = intent.GetBooleanExtra("IsVibrateOn", false);
            IsCountOn = intent.GetBooleanExtra("IsCountOn", false);
            IsCountSoundOn = intent.GetBooleanExtra("IsCountSoundOn", false);
            IsRepeating = intent.GetBooleanExtra("IsRepeating", false);
            toneName = intent.GetStringExtra("toneName");
            alarmVolume = intent.GetIntExtra("alarmVolume", 0);

            StartAlarmActivity(context);
        }

        private void StartAlarmActivity(Context context)
        {
            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtra("id", id);
            disIntent.PutExtra("name", name);
            disIntent.PutExtra("IsAlarmOn", IsAlarmOn);
            disIntent.PutExtra("IsVibrateOn", IsVibrateOn);
            disIntent.PutExtra("IsCountOn", IsCountOn);
            disIntent.PutExtra("IsCountSoundOn", IsCountSoundOn);
            disIntent.PutExtra("IsRepeating", IsRepeating);
            disIntent.PutExtra("toneName", toneName);
            disIntent.PutExtra("alarmVolume", alarmVolume);

            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
        }
    }
}