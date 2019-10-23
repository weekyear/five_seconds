using Five_Seconds.Models;
using System;
using System.Collections.Generic;

namespace Five_Seconds.Repository
{
    public class AlarmsRepository : IAlarmsRepository
    {
        public ItemDatabaseGeneric ItemDatabase { get; }

        public AlarmsRepository(ItemDatabaseGeneric itemDatabase)
        {
            //if (Device.RuntimePlatform == "Test") return;
            ItemDatabase = itemDatabase;
        }

        public List<Alarm> AlarmsFromDB
        {
            get { return GetAllAlarms() as List<Alarm>; }
        }
        public List<DaysOfWeek> DaysOfWeeksFromDB
        {
            get { return GetAllDaysOfWeeks() as List<DaysOfWeek>; }
        }
        public List<Record> RecordFromDB
        {
            get { return GetAllRecords() as List<Record>; }
        }

        // Alarm

        public Alarm GetAlarm(int id)
        {
            return ItemDatabase.GetObject<Alarm>(id);
        }

        public IEnumerable<Alarm> GetAllAlarms()
        {
            Console.WriteLine("GetAllAlarms_AlarmsRepository");
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

        public Record GetRecord(int id)
        {
            return ItemDatabase.GetObject<Record>(id);
        }

        private IEnumerable<Record> GetAllRecords()
        {
            return ItemDatabase.GetObjects<Record>();
        }

        public int SaveRecord(Record record)
        {
            return ItemDatabase.SaveObject(record);
        }

        public int DeleteRecord(int id)
        {
            return ItemDatabase.DeleteObject<Record>(id);
        }

        public void DeleteAllRecords()
        {
            ItemDatabase.DeleteAllObjects<Record>();
        }
    }
}
