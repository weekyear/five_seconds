using Five_Seconds.Models;
using Five_Seconds.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Repository
{
    public interface IAlarmRepository
    {
        ItemDatabaseGeneric ItemDatabase { get; }

        Alarm GetAlarm(int id);
        IEnumerable<Alarm> GetAllAlarms();
        //IEnumerable<Alarm> GetTodaysAlarms();

        int SaveAlarm(Alarm alarm);
        //void UpdateAlarm(Alarm alarm);
        int DeleteAlarm(Alarm alarm);
        bool DoesAlarmExist(Alarm alarm);
        void DeleteAllAlarms();

        //Settings GetSettings();
    }
}
