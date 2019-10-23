using Five_Seconds.Models;
using Five_Seconds.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Services
{
    public class AlarmService : IAlarmService
    {
        public IAlarmsRepository Repository { get; }
        public List<Alarm> Alarms { get; set; } = new List<Alarm>();

        public AlarmService(IAlarmsRepository repository)
        {
            Console.WriteLine("Start_AlarmService");
            Repository = repository;

            Alarms = GetAllAlarms();
        }

        public List<Alarm> GetAllAlarms()
        {
            Console.WriteLine("GetAllAlarms_AlarmService");
            return AssignDaysToAlarms();
        }

        private List<Alarm> AssignDaysToAlarms()
        {
            Console.WriteLine("AssignDaysToAlarms_AlarmService");
            var alarms = Repository.AlarmsFromDB;
            Console.WriteLine("AlarmsFromDB_AlarmService");
            var daysOfWeeks = Repository.DaysOfWeeksFromDB;
            Console.WriteLine("DaysOfWeeksFromDB_AlarmService");

            foreach (var days in daysOfWeeks)
            {
                for (int i = 0; i < alarms.Count; i++)
                {
                    if (alarms[i].DaysId == days.Id)
                    {
                        alarms[i].Days = days;
                    }
                }
            }

            Console.WriteLine($"AssignAlarms Count :: {alarms.Count}");

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
            Repository.DeleteDaysOfWeek(alarm.Id);
            
            foreach(var m in Alarms)
            {
                if (m.Id == alarm.Id)
                {
                    Alarms.Remove(m);
                    break;
                }
            }

            SendChangeAlarmsMessage();
            return id;
        }

        public int TurnOffAlarm(Alarm alarm)
        {
            DependencyService.Get<IAlarmSetter>().DeleteAlarm(alarm.Id);
            return SaveAlarmAtLocal(alarm);
        }

        public int SaveAlarm(Alarm alarm)
        {
            var id = SaveAlarmAtLocal(alarm);
            DependencyService.Get<IAlarmSetter>().SetAlarm(alarm);
            return id;
        }

        public int SaveAlarmAtLocal(Alarm alarm)
        {
            alarm.DaysId = Repository.SaveDaysOfWeek(alarm.Days);
            Repository.SaveAlarm(alarm);
            UpdateAlarms();
            SendChangeAlarmsMessage();
            return alarm.Id;
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
            DateTime min = DateTime.MaxValue;

            if (Alarms.Count == 0) return null;

            var nextAlarm = new Alarm() { Date = DateTime.MaxValue.Date };

            for (int i = 0; i < Alarms.Count; i++)
            {
                if (Alarms[i].IsActive)
                {
                    var alarmNextTime = Alarms[i].NextAlarmTime;

                    if (min.Subtract(alarmNextTime).TotalMilliseconds > 0)
                    {
                        min = alarmNextTime;
                        nextAlarm = Alarms[i];
                    }
                }
            }

            if (nextAlarm.Date == DateTime.MaxValue.Date)
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
