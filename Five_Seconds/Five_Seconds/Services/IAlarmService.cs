using Five_Seconds.Models;
using Five_Seconds.Repository;
using System.Collections.ObjectModel;

namespace Five_Seconds.Services
{
    public interface IAlarmService
    {
        IAlarmsRepository Repository { get; }
        ObservableCollection<Alarm> Alarms { get; }
        Alarm GetAlarm(int id);
        int DeleteAlarm(Alarm alarm);
        int SaveAlarm(Alarm alarm);
        int SaveAlarmAtLocal(Alarm alarm);
        void DeleteAllAlarms();
        void SendChangeAlarmsMessage();
        ObservableCollection<Alarm> GetAllAlarms();
        Alarm GetNextAlarm();
    }
}
