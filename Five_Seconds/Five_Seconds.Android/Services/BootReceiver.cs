using System;
using Android;
using Android.App;
using Android.Content;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_BootReceiver");
            if (intent.Action.Equals(Intent.ActionBootCompleted))
            {
                //AlarmController.SetAllAlarmWhenRestart();
                Console.WriteLine("Finish SetAllAlarmWhenRestart_BootReceiver");
            }
        }
    }
}