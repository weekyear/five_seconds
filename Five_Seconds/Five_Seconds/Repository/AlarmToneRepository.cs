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

        public AlarmTone GetTone(int id)
        {
            return ItemDatabase.GetObject<AlarmTone>(id);
        }

        public List<AlarmTone> GetAllAlarmTones()
        {
            var defaultTones = new List<AlarmTone>()
            {
                new AlarmTone("Buzz", "buzz.mp3"),
                new AlarmTone("Synth", "synth.mp3"),
                new AlarmTone("Xylophone", "xylophone.mp3"),
                new AlarmTone("Shooting Stars", "shooting_stars.mp3"),
                new AlarmTone("Sixteen Bit", "sixteen_bit.mp3"),
                new AlarmTone("Sci-fi", "sci_fi.mp3")
            };

            var allTones = GetAllTones() as List<AlarmTone>;

            allTones.ForEach((tone) => defaultTones.Add(tone));

            return defaultTones;
        }
    }
}
