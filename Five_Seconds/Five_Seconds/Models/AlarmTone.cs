using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("AlarmTone")]
    public class AlarmTone : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsCustomTone { get; set; } = false;

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
    }
}
