﻿using Five_Seconds.Models;
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
        public ObservableCollection<Alarm> Alarms { get; set; } = new ObservableCollection<Alarm>();

        public AlarmService(IAlarmsRepository repository)
        {
            Repository = repository;

            Alarms = GetAllAlarms();
        }

        public ObservableCollection<Alarm> GetAllAlarms()
        {
            var alarms = AssignDaysToAlarms();
            return ConvertListToObservableCollection(alarms);
        }

        private List<Alarm> AssignDaysToAlarms()
        {
            var alarms = Repository.AlarmsFromDB;

            foreach (var days in Repository.DaysOfWeeksFromDB)
            {
                for (int i = 0; i < alarms.Count; i++)
                {
                    if (alarms[i].DaysId == days.Id)
                    {
                        alarms[i].Days = days;
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
            //DependencyService.Get<IAlarmNotification>().UpdateNotification();
            return id;
        }

        public int SaveAlarm(Alarm alarm)
        {
            var id = SaveAlarmAtLocal(alarm);
            DependencyService.Get<IAlarmSetter>().SetAlarm(alarm);
            //DependencyService.Get<IAlarmNotification>().UpdateNotification();
            return id;
        }

        public int SaveAlarmAtLocal(Alarm alarm)
        {
            alarm.DaysId = Repository.SaveDaysOfWeek(alarm.Days);
            Repository.SaveAlarm(alarm);
            UpdateAlarms();
            //AddOrModifyAlarmToAlarms(alarm);
            SendChangeAlarmsMessage();
            return alarm.Id;
        }

        private void UpdateAlarms()
        {
            Alarms = GetAllAlarms();
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

        private ObservableCollection<T> ConvertListToObservableCollection<T>(List<T> list)
        {
            var collection = new ObservableCollection<T>();
            foreach (var item in list)
            {
                collection.Add(item);
            }
            return collection;
        }
    }
}