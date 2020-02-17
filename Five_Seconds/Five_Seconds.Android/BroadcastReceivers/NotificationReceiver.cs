using System;
using Android.App;
using Android.Content;
using Android.OS;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Five_Seconds.Droid.Services;
using Plugin.CurrentActivity;
using Five_Seconds.Helpers;
using Five_Seconds.ViewModels;

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
            GetAlarmById();

            try
            {
                if (intent.Action == context.GetString(Resource.String.AlarmOff))
                {
                    TurnOffLaterAlarm();

                    CreateFailedRecord();

                    if (HelperAndroid.IsApplicationInTheBackground())
                    {
                        CrossCurrentActivity.Current.Activity?.FinishAffinity();
                    }
                }
                else if (intent.Action == context.GetString(Resource.String.GoOffNow))
                {
                    OpenAlarmActivity(context, bundle);
                }
                else if (intent.Action == context.GetString(Resource.String.AlarmPreOff))
                {
                    alarm.IsTurnOffPreAlarm = true;
                    alarm.IsGoOffPreAlarm = true;
                    App.AlarmService.SaveAlarmAtLocal(alarm);

                    TurnOffPreAlarm();

                    if (HelperAndroid.IsApplicationInTheBackground()) CrossCurrentActivity.Current.Activity?.FinishAffinity();
                }
                else if (intent.Action == "GoOffPre")
                {
                    alarm.IsGoOffPreAlarm = true;
                    App.AlarmService.SaveAlarmAtLocal(alarm);

                    alarm.LaterAlarmTime = DateTime.Now.AddSeconds(0.5);
                    AlarmHelper.SetLaterAlarm(alarm);
                }
            }
            catch { }
            finally
            {
                CancelNotification(context, intent);
                AlarmsViewModel.RefreshAlarmsCommand?.Execute(null);
            }
        }

        private void TurnOffLaterAlarm()
        {
            if (!DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                Alarm.ChangeIsActive(alarm, false);
            }

            TurnOffLaterAlarmByNotification(alarm);
        }
        
        private void TurnOffPreAlarm()
        {
            if (!DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                alarm.IsActive = false;
                TurnOffAlarmByNotification(alarm);
            }
            else
            {
                AlarmHelper.SetAlarmAtFirst(alarm);
            }
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

        private void TurnOffLaterAlarmByNotification(Alarm alarm)
        {
            alarm.IsLaterAlarm = false; 
            TurnOffAlarmByNotification(alarm);
        }
        
        private void TurnOffAlarmByNotification(Alarm alarm)
        {
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

        private void GetAlarmById()
        {
            alarmService = App.AlarmService;
            Alarm.IsInitFinished = false;
            alarm = alarmService.GetAlarm(id);
            Alarm.IsInitFinished = true;
        }
    }
}