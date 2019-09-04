using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Five_Seconds.Models
{
    [Table("Mission")]
    public class Mission : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Percentage { get; set; }
        public bool IsActive { get; set; } = true;

        [OneToMany]
        public List<Record> Records { get; set; } = new List<Record>();

        //public int AlarmId { get; set; }
        [OneToOne]
        public Alarm Alarm { get; set; } = new Alarm();

        public Mission() { }

        public Mission (Mission original)
        {
            Id = original.Id;
            Name = original.Name;
            Alarm = original.Alarm;
            Percentage = original.Percentage;
            IsActive = original.IsActive;
        }
    }
}