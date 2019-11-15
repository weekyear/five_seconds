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
    public class ComebackReceiver : BroadcastReceiver
    {
        private int id;
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_NotificationReceiver");
            var bundle = intent.Extras;

            id = bundle.GetInt("id", -100000);

            if (id == -98)
            {
                NotificationAndroid.NotifyAppComeback();
            }
            else if (id == -99)
            {
                NotificationAndroid.NotifyAlarmComeback();
            }
        }
    }
}