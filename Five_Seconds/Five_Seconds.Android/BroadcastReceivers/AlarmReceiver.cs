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
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_AlarmReceiver");
            var bundle = intent.Extras;
            var id = (int)bundle.Get("id");
            Console.WriteLine($"Id_AlarmReceiver : {id}");

            NotificationAndroid.CancelLaterNotification(context, id);

            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtras(bundle);

            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
        }
    }
}