using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Repository
{
    public class MissionsRepository : IMissionsRepository
    {
        public ItemDatabaseGeneric ItemDatabase { get; } = App.ItemDatabase;

        public List<Mission> MissionsFromDB { get; set; }
        public List<Alarm> AlarmsFromDB { get; set; }
        public List<DaysOfWeek> DaysOfWeeksFromDB { get; set; }
        public List<Record> RecordFromDB { get; set; }

        public MissionsRepository()
        {
            if (Device.RuntimePlatform == "Test") return;

            MissionsFromDB = GetAllMissions() as List<Mission>;
            AlarmsFromDB = GetAllAlarms() as List<Alarm>;
            DaysOfWeeksFromDB = GetAllDaysOfWeeks() as List<DaysOfWeek>;
            RecordFromDB = GetAllRecords() as List<Record>;
        }

        // Mission

        public Mission GetMission(int id)
        {
            return ItemDatabase.GetObject<Mission>(id);
        }

        public IEnumerable<Mission> GetFirstMissions()
        {
            return ItemDatabase.GetObjects<Mission>();
        }

        public IEnumerable<Mission> GetAllMissions()
        {
            return ItemDatabase.GetObjects<Mission>();
        }

        public int SaveMission(Mission mission)
        {
            return ItemDatabase.SaveObject(mission);
        }

        public int DeleteMission(int id)
        {
            return ItemDatabase.DeleteObject<Mission>(id);
        }

        public void DeleteAllMissions()
        {
            ItemDatabase.DeleteAllObjects<Mission>();
        }

        // Alarm

        public Alarm GetAlarm(int id)
        {
            return ItemDatabase.GetObject<Alarm>(id);
        }

        public IEnumerable<Alarm> GetAllAlarms()
        {
            return ItemDatabase.GetObjects<Alarm>();
        }

        public int SaveAlarm(Alarm alarm)
        {
            return ItemDatabase.SaveObject(alarm);
        }
        public int DeleteAlarm(int id)
        {
            return ItemDatabase.DeleteObject<Alarm>(id);
        }

        public void DeleteAllAlarms()
        {
            ItemDatabase.DeleteAllObjects<Alarm>();
        }

        // DaysOfWeek

        public DaysOfWeek GetDaysOfWeek(int id)
        {
            return ItemDatabase.GetObject<DaysOfWeek>(id);
        }

        public IEnumerable<DaysOfWeek> GetAllDaysOfWeeks()
        {
            return ItemDatabase.GetObjects<DaysOfWeek>();
        }

        public int SaveDaysOfWeek(DaysOfWeek daysOfWeek)
        {
            return ItemDatabase.SaveObject(daysOfWeek);
        }

        public int DeleteDaysOfWeek(int id)
        {
            return ItemDatabase.DeleteObject<DaysOfWeek>(id);
        }

        public void DeleteAllDaysOfWeeks()
        {
            ItemDatabase.DeleteAllObjects<DaysOfWeek>();
        }

        // Record

        private Record GetRecord(int id)
        {
            return ItemDatabase.GetObject<Record>(id);
        }

        private IEnumerable<Record> GetAllRecords()
        {
            return ItemDatabase.GetObjects<Record>();
        }

        public int SaveRecords(Record record)
        {
            return ItemDatabase.SaveObject(record);
        }

        public int DeleteRecords(int id)
        {
            return ItemDatabase.DeleteObject<Record>(id);
        }

        public void DeleteAllRecords()
        {
            ItemDatabase.DeleteAllObjects<Record>();
        }
    }
}
