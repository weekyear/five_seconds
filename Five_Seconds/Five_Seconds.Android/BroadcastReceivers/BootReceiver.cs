using System;
using Android;
using Android.App;
using Android.Content;
using Five_Seconds.Droid.Services;

namespace Five_Seconds.Droid.BroadcastReceivers
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(Intent.ActionBootCompleted))
            {
                AlarmHelper.SetAllAlarmWhenRestart();
            }
        }
    }
}