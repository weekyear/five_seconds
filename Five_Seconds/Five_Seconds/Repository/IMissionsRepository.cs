using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Five_Seconds.Repository
{
    public interface IMissionsRepository
    {
        // getters
        List<Mission> MissionsFromDB { get; }
        List<Alarm> AlarmsFromDB { get; }
        List<DaysOfWeek> DaysOfWeeksFromDB { get; }
        List<Record> RecordFromDB { get; }

        // methods
        // Mission

        Mission GetMission(int id);
        IEnumerable<Mission> GetAllMissions();
        int SaveMission(Mission mission);

        int DeleteMission(int id);

        void DeleteAllMissions();

        // Alarm
        Alarm GetAlarm(int id);
        IEnumerable<Alarm> GetAllAlarms();
        int SaveAlarm(Alarm alarm);

        int DeleteAlarm(int id);

        void DeleteAllAlarms();

        // DayOfWeek
        DaysOfWeek GetDaysOfWeek(int id);
        IEnumerable<DaysOfWeek> GetAllDaysOfWeeks();

        int SaveDaysOfWeek(DaysOfWeek daysOfWeek);

        int DeleteDaysOfWeek(int id);

        void DeleteAllDaysOfWeeks();

        // Record

        int SaveRecords(Record record);

        int DeleteRecords(int id);

        void DeleteAllRecords();

        // NextAlarmId
        int GetNextAlarmId();
    }
}
