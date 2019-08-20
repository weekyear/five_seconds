﻿using System;
using System.Collections.Generic;
using System.Text;
using Five_Seconds.Models;
using Five_Seconds.Repository;

namespace Five_Seconds.Repository
{
    public class AlarmRepository : IAlarmRepository
    {
        public ItemDatabaseGeneric ItemDatabase { get; } = MissionsRepository.ItemDatabase;


        public Alarm GetAlarm(int id)
        {
            return ItemDatabase.GetObject<Alarm>(id);
        }

        public int SaveAlarm(Alarm alarm)
        {
            return ItemDatabase.SaveObject<Alarm>(alarm);
        }

        public int DeleteAlarm(Alarm alarm)
        {
            return ItemDatabase.DeleteObject<Alarm>(alarm.Id);
        }

        public void DeleteAllAlarms()
        {
            ItemDatabase.DeleteAllObjects<Alarm>();
        }

        public bool DoesAlarmExist(Alarm alarm)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Alarm> GetAllAlarms()
        {
            return ItemDatabase.GetObjects<Alarm>();
        }

        //public List<Alarm> GetTodaysAlarms()
        //{
        //    var all = ItemDatabase.GetObjects<Alarm>() as List<Alarm>;
        //    return all.Where(x => x.OccursToday == true).ToList();
        //}
    }
}
