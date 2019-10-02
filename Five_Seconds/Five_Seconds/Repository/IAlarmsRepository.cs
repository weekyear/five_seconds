using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Five_Seconds.Repository
{
    public interface IAlarmsRepository
    {
        // getters
        List<Alarm> AlarmsFromDB { get; }
        List<DaysOfWeek> DaysOfWeeksFromDB { get; }
        List<Record> RecordFromDB { get; }

        // Alarm

        Alarm GetAlarm(int id);
        int SaveAlarm(Alarm alarm);
        IEnumerable<Alarm> GetAllAlarms();

        int DeleteAlarm(int id);

        void DeleteAllAlarms();

        // DayOfWeek
        DaysOfWeek GetDaysOfWeek(int id);
        IEnumerable<DaysOfWeek> GetAllDaysOfWeeks();

        int SaveDaysOfWeek(DaysOfWeek daysOfWeek);

        int DeleteDaysOfWeek(int id);

        void DeleteAllDaysOfWeeks();

        // Record
        Record GetRecord(int id);
        int SaveRecord(Record record);
        int DeleteRecord(int id);
        void DeleteAllRecords();

        // NextAlarmId

    }
}
