using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("AlarmTone")]
    public class AlarmTone : IObject
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsCustomTone { get; set; }

        public AlarmTone()
        {
            Name = string.Empty;
            Path = string.Empty;
            IsCustomTone = false;
        }

        public AlarmTone(string name, string path)
        {
            Name = name;
            Path = path;
            IsCustomTone = false;
        }

        public override bool Equals(object obj)
        {
            if (this is null || obj is null) return false;

            if (!(obj is AlarmTone)) return false;

            var otherTone = (AlarmTone)obj;
            if (Id == otherTone.Id)
                return true;

            if (Name == otherTone.Name && Path == otherTone.Path)
                return true;

            return false;
        }

        public static readonly List<AlarmTone> Tones = new List<AlarmTone>()
        {
            new AlarmTone("Buzz", "buzz.mp3"),
            new AlarmTone("Synth", "synth.mp3"),
            new AlarmTone("Xylophone", "xylophone.mp3"),
            new AlarmTone("Shooting Stars", "shooting_stars.mp3"),
            new AlarmTone("Sixteen Bit", "sixteen_bit.mp3"),
            new AlarmTone("Sci-fi", "sci_fi.mp3")
        };
    }
}
