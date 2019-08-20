using System;
using System.Collections.Generic;
using System.Text;
using Five_Seconds.Models;
using Five_Seconds.Repository;

namespace Five_Seconds.Services
{
    public class AlarmStorageService : IAlarmStorageService
    {
        public ItemDatabaseGeneric ItemDatabase { get; } = LocalData.ItemDatabase; 

        public void AddAlarm(Alarm alarm)
        {
            throw new NotImplementedException();
        }

        public void DeleteAlarm(Alarm alarm)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllAlarms()
        {
            throw new NotImplementedException();
        }

        public bool DoesAlarmExist(Alarm alarm)
        {
            throw new NotImplementedException();
        }

        public Alarm GetAlarm(string id)
        {
            throw new NotImplementedException();
        }

        public List<Alarm> GetAllAlarms()
        {
            throw new NotImplementedException();
        }

        public List<Alarm> GetTodaysAlarms()
        {
            throw new NotImplementedException();
        }

        public void UpdateAlarm(Alarm alarm)
        {
            throw new NotImplementedException();
        }
    }
}
