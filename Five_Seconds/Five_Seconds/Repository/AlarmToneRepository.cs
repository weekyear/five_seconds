using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Five_Seconds.Models;
using Five_Seconds.Repository;

namespace Five_Seconds.Repository
{
    public class AlarmToneRepository : IAlarmToneRepository
    {
        public ItemDatabaseGeneric ItemDatabase { get; } = App.ItemDatabase;

        public IEnumerable<AlarmTone> GetAllTones()
        {
            return ItemDatabase.GetObjects<AlarmTone>();
        }

        public int AddTone(AlarmTone alarmTone)
        {
            return ItemDatabase.SaveObject(alarmTone);
        }

        public int DeleteTone(AlarmTone alarmTone)
        {
            return ItemDatabase.DeleteObject<AlarmTone>(alarmTone.Id);
        }

        public void SetDefaultTones()
        {
            var tones = AlarmTone.Tones;

            foreach (AlarmTone tone in tones)
            {
                ItemDatabase.SaveObject(tone);
            }
        }

        public AlarmTone GetTone(int id)
        {
            return ItemDatabase.GetObject<AlarmTone>(id);
        }

        //public List<Alarm> GetTodaysAlarms()
        //{
        //    var all = ItemDatabase.GetObjects<Alarm>() as List<Alarm>;
        //    return all.Where(x => x.OccursToday == true).ToList();
        //}
    }
}
