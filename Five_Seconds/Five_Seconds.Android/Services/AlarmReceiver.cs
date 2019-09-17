using Android.Content;
using Five_Seconds.Models;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public static Mission mission;
        public override void OnReceive(Context context, Intent intent)
        {
            var id = intent.GetIntExtra("id", 0);

            if (App.MissionsRepo != null)
            {
                mission = App.MissionsRepo.GetMission(id);
                mission.Alarm = App.MissionsRepo.GetAlarm(mission.AlarmId);
                mission.Alarm.Days = App.MissionsRepo.GetDaysOfWeek(mission.Alarm.DaysId);
            }
            else
            {
                return;
            }

            if (mission.IsActive)
            {
                AlarmController.SetNextAlarm(mission);
                StartAlarmActivity(context, id);
            }
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