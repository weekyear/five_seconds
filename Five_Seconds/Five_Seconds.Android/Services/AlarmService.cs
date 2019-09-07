using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Five_Seconds.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace Five_Seconds.Droid.Services
{
    [Service]
    public class AlarmService : Service
    {
        public override void OnCreate()
        {

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                StartMyOwnForeground();
            else
                StartForeground(2, new Notification());

            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            SetAlarmManager(intent);

            return StartCommandResult.Sticky;
        }

        private void StartMyOwnForeground()
        {
            var NOTIFICATION_CHANNEL_ID = "com.beside.five_seconds";
            var channelName = "my_5seconds_alarm";
            var chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName, NotificationImportance.Low);
            var manager = (NotificationManager)GetSystemService(NotificationService);

            manager?.CreateNotificationChannel(chan);

            var notification = AlarmNotificationAndroid.GetNextAlarmNotification(this);

            StartForeground(2, notification);
        }

        private void SetAlarmManager(Intent intent)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OPEN THE THING");
            var id = intent.GetIntExtra("id", 0);
            var diffMillis = intent.GetLongExtra("diffMillis", 0);

            SetAlarmByManager(id, diffMillis);
            Log.Debug(AlarmSetterAndroid.AlarmTag, "START ACTIVITY");
        }

        private void SetAlarmByManager(int id, long diffMillis)
        {
            if (diffMillis == 0) return;
            var _alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(AlarmService);

            alarmManager.SetExact(AlarmType.RtcWakeup, diffMillis, pendingIntent);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}