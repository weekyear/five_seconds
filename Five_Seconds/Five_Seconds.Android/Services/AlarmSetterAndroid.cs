﻿using System;
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
using Five_Seconds.Droid.BroadcastReceivers;
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
        public AlarmSetterAndroid()
        {

        }

        public void SetAlarm(Alarm alarm)
        {
            AlarmController.SetFirstAlarm(alarm);
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

        public void DeleteAllAlarms(List<Alarm> alarms)
        {
            alarms.ForEach((alarm) => DeleteAlarm(alarm.Id));
        }
    }
}