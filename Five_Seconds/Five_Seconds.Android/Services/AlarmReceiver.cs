using Android.Content;
using Five_Seconds.Models;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public static Mission AlarmMissionNow;
        public override void OnReceive(Context context, Intent intent)
        {
            var id = intent.GetIntExtra("id", 0);

            StartAlarmActivity(context, id);
        }

        private void StartAlarmActivity(Context context, int id)
        {
            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtra("id", id);
            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
        }
    }
}