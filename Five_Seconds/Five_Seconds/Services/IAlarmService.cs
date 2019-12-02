using Five_Seconds.Models;
using Five_Seconds.Repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Five_Seconds.Services
{
    public interface IAlarmService
    {
        IAlarmsRepository Repository { get; }
        List<Alarm> Alarms { get; }
        Alarm GetAlarm(int id);
        int DeleteAlarm(Alarm alarm);
        int SaveAlarm(Alarm alarm);
        void SaveAlarmsAtLocal(IEnumerable<Alarm> alarms);
        int SaveAlarmAtLocal(Alarm alarm);
        void DeleteAllAlarms();
        int TurnOffAlarm(Alarm alarm);
        void SendChangeAlarmsMessage();
        List<Alarm> GetAllAlarms();
        Alarm GetNextAlarm();
    }
}
