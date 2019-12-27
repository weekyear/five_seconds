using Five_Seconds.Models;
using Five_Seconds.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Five_Seconds.Services
{
    public class AlarmService : IAlarmService
    {
        public IAlarmsRepository Repository { get; }
        public IEnumerable<Alarm> Alarms { get; private set; } = new List<Alarm>();

        public AlarmService(IAlarmsRepository repository)
        {
            Repository = repository;

            UpdateAlarms();
        }

        public IEnumerable<Alarm> GetAllAlarms()
        {
            return AssignDaysToAlarms();
        }

        private IEnumerable<Alarm> AssignDaysToAlarms()
        {
            var alarms = Repository.AlarmsFromDB;
            var daysOfWeeks = Repository.DaysOfWeeksFromDB;

            foreach (var days in daysOfWeeks)
            {
                foreach(var alarm in alarms)
                {
                    if (alarm.DaysId == days.Id)
                    {
                        alarm.Days = days;
                    }
                }
            }

            return alarms;
        }

        public Alarm GetAlarm(int id)
        {
            return Alarms.FirstOrDefault(m => m.Id == id);
        }

        public int DeleteAlarm(Alarm alarm)
        {
            DependencyService.Get<IAlarmSetter>().DeleteAlarm(alarm.Id);
            var id = Repository.DeleteAlarm(alarm.Id);
            Repository.DeleteDaysOfWeek(alarm.DaysId);

            UpdateAlarms();
            SendChangeAlarmsMessage();
            return id;
        }

        public int TurnOffAlarm(Alarm alarm)
        {
            DependencyService.Get<IAlarmSetter>().DeleteAlarm(alarm.Id) ;

            alarm.IsLaterAlarm = false;
            return SaveAlarmAtLocal(alarm);
        }

        public int SaveAlarm(Alarm alarm)
        {
            var id = SaveAlarmAtLocal(alarm);
            DependencyService.Get<IAlarmSetter>().SetAlarm(alarm);
            return id;
        }

        public void SaveAlarmsAtLocal(IEnumerable<Alarm> alarms)
        {
            foreach(var alarm in alarms)
            {
                alarm.DaysId = Repository.SaveDaysOfWeek(alarm.Days);
                var id = Repository.SaveAlarm(alarm);
            }
            UpdateAlarms();
            SendChangeAlarmsMessage();
        }

        public int SaveAlarmAtLocal(Alarm alarm)
        {
            alarm.DaysId = Repository.SaveDaysOfWeek(alarm.Days);
            var id = Repository.SaveAlarm(alarm);
            UpdateAlarms();
            SendChangeAlarmsMessage();

            return id;
        }

        private void UpdateAlarms()
        {
            Alarm.IsInitFinished = false;
            Alarms = GetAllAlarms();
            Alarm.IsInitFinished = true;
        }

        public void DeleteAllAlarms()
        {
            foreach (var alarm in Alarms)
            {
                DeleteAlarm(alarm);
            }
        }

        public Alarm GetNextAlarm()
        {
            DateTime min = DateTimeOffset.MaxValue.Date;

            if (Alarms.Count() == 0) return null;

            var nextAlarm = new Alarm() { Date = DateTimeOffset.MaxValue.Date };

            foreach (var alarm in Alarms)
            {
                if (alarm.IsActive)
                {
                    DateTime alarmNextTime;
                    if (alarm.IsLaterAlarm)
                    {
                        alarmNextTime = alarm.LaterAlarmTime;
                    }
                    else
                    {
                        alarmNextTime = alarm.NextAlarmTime;
                    }

                    if (min.Subtract(alarmNextTime).TotalMilliseconds > 0)
                    {
                        min = alarmNextTime;
                        nextAlarm = alarm;
                    }
                }
            }

            if (nextAlarm.Date == DateTimeOffset.MaxValue.Date)
            {
                return null;
            }
            return nextAlarm;
        }

        public void SendChangeAlarmsMessage()
        {
            MessagingCenter.Send(this, "changeAlarms");
        }
    }
}
