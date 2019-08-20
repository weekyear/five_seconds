using Five_Seconds.Models;
using Five_Seconds.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Services
{
    public interface IAlarmStorageService
    {
        ItemDatabaseGeneric ItemDatabase { get; }

        Alarm GetAlarm(string id);
        List<Alarm> GetAllAlarms();
        List<Alarm> GetTodaysAlarms();

        void AddAlarm(Alarm alarm);
        void UpdateAlarm(Alarm alarm);
        void DeleteAlarm(Alarm alarm);
        bool DoesAlarmExist(Alarm alarm);
        void DeleteAllAlarms();

        //Settings GetSettings();
    }
}
