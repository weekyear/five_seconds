using System;

using Android.App;
using Android.Content;
using Android.OS;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using SQLite;
using static Android.App.ActivityManager;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    public class NotificationReceiver : BroadcastReceiver
    {
        private IAlarmService alarmService;
        private int id;
        private SQLiteConnection sqliteConnection;
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_NotificationReceiver");
            var bundle = intent.Extras;

            id = bundle.GetInt("id", -100000);

            CancelNotification(context, intent);

            switch (intent.Action)
            {
                case "알람 해제":
                    GetAlarmService();
                    Alarm.IsInitFinished = false;
                    // 서비스에서 TurnOffAlarm();
                    var alarm = alarmService.GetAlarm(id);

                    alarm.IsActive = false;

                    TurnOffAlarm(alarm);

                    Alarm.IsInitFinished = true;

                    if (IsApplicationInTheBackground())
                    {
                        CrossCurrentActivity.Current.Activity.FinishAffinity();
                    }
                    break;
                case "지금 울림":
                    OpenAlarmActivity(context, bundle);
                    break;
            }


        }

        private void GetAlarmService()
        {
            try
            {
                alarmService = App.Service;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
                Console.WriteLine("App.Service == null_NotificationReceiver");
                alarmService = CreateServiceWithoutCore();
            }
        }

        private void OpenAlarmActivity(Context context, Bundle bundle)
        {
            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtras(bundle);

            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
        }

        private void TurnOffAlarm(Alarm alarm)
        {
            try
            {
                alarmService.TurnOffAlarm(alarm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
                Console.WriteLine("TurnOffAlarm_Background_NotificationReceiver");
                TurnOffAlarmWhenIsBackground(alarm);
            }
        }

        private void TurnOffAlarmWhenIsBackground(Alarm alarm)
        {
            alarm.IsLaterAlarm = false;
            var alarmSetter = new AlarmSetterAndroid();
            alarmSetter.DeleteAlarm(alarm.Id);
            alarmService.SaveAlarmAtLocal(alarm);
        }

        private void CancelNotification(Context context, Intent intent)
        {
            var extras = intent.Extras;
            if (extras != null && !extras.IsEmpty)
            {
                NotificationManager manager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (id != -100000)
                {
                    Console.WriteLine("CancelNotification_NotificationReceiver");
                    manager.Cancel(id);
                }
            }
        }

        private bool IsApplicationInTheBackground()
        {
            bool isInBackground;

            RunningAppProcessInfo myProcess = new RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(myProcess);
            isInBackground = myProcess.Importance != Importance.Foreground;

            return isInBackground;
        }


        private AlarmService CreateServiceWithoutCore()
        {
            var deviceStorage = new DeviceStorageAndroid();
            sqliteConnection = new SQLiteConnection(deviceStorage.GetFilePath("AlarmsSQLite.db3"));
            var itemDatabase = new ItemDatabaseGeneric(sqliteConnection);
            var alarmsRepo = new AlarmsRepository(itemDatabase);
            var service = new AlarmService(alarmsRepo);

            return service;
        }
    }
}