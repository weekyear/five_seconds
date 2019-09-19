﻿using Five_Seconds.Models;
using System;
using System.Collections.Generic;

namespace Five_Seconds.Repository
{
    public class MissionsRepository : IMissionsRepository
    {
        public ItemDatabaseGeneric ItemDatabase { get; }

        public MissionsRepository(ItemDatabaseGeneric itemDatabase)
        {
            //if (Device.RuntimePlatform == "Test") return;
            Console.WriteLine("Constructor_MissionsRepository");
            ItemDatabase = itemDatabase;
        }

        public List<Mission> MissionsFromDB
        {
            get { return GetAllMissions() as List<Mission>; }
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

        // Mission

        public Mission GetMission(int id)
        {
            Console.WriteLine("GetMission");
            var mission = ItemDatabase.GetObject<Mission>(id);
            if (mission != null)
            {
                Console.WriteLine(mission.Name);
            }
            else
            {
                Console.WriteLine("mission is null");
            }
            return mission;
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
