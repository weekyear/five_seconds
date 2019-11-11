using System;
using Android.App;
using Android.Content;
using Android.OS;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.Droid.Services;
using Plugin.CurrentActivity;
using SQLite;
using static Android.App.ActivityManager;

namespace Five_Seconds.Droid.BroadcastReceivers
{
    [BroadcastReceiver]
    public class NotificationReceiver : BroadcastReceiver
    {
        private IAlarmService alarmService;
        private Alarm alarm;
        private int id;
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_NotificationReceiver");
            var bundle = intent.Extras;

            id = bundle.GetInt("id", -100000);

            CancelNotification(context, intent);

            switch (intent.Action)
            {
                case "알람 해제":
                    TurnOffLaterAlarm();

                    CreateFailedRecord();

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

        private void TurnOffLaterAlarm()
        {
            Console.WriteLine("TurnOffLaterAlarm_00");
            alarmService = HelperAndroid.GetAlarmService();
            Alarm.IsInitFinished = false;
            Console.WriteLine("TurnOffLaterAlarm_01");
            // 서비스에서 TurnOffAlarm();
            Console.WriteLine("TurnOffLaterAlarm_02");
            alarm = alarmService.GetAlarm(id);

            if (!DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                alarm.IsActive = false;
            }

            Console.WriteLine("TurnOffLaterAlarm_03");
            TurnOffAlarm(alarm);

            Alarm.IsInitFinished = true;
        }

        private void CreateFailedRecord()
        {
            alarm.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            var record = new Record(alarm, false);
            alarmService.Repository.SaveRecord(record);
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
            SaveAlarmAtLocal();
        }

        private void SaveAlarmAtLocal()
        {
            alarmService.Repository.SaveDaysOfWeek(alarm.Days);
            alarmService.Repository.SaveAlarm(alarm);
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
            GetMyMemoryState(myProcess);
            isInBackground = myProcess.Importance != Importance.Foreground;

            return isInBackground;
        }
    }
}