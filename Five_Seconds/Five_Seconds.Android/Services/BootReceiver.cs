using System;

using Android.App;
using Android.Content;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_BootReceiver");
            if (intent.Action.Equals("android.intent.action.BOOT_COMPLETED"))
            {
                Console.WriteLine("Start_SetAllAlarmWhenRestart");
                AlarmController.SetAllAlarmWhenRestart();
            }
        }
    }
}