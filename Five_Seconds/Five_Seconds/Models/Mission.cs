using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
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
        public int TimeLimit { get; set; } = 5;

        [OneToMany]
        public ObservableCollection<Record> Records { get; set; } = new ObservableCollection<Record>();

        [OneToOne]
        public Alarm Alarm { get; set; } = new Alarm();
        public TimeSpan Time
        {
            get { return Alarm.Time; }
        }

        public Mission() { }

        public Mission (Mission original)
        {
            Id = original.Id;
            Name = original.Name;
            Alarm = original.Alarm;
            Percentage = original.Percentage;
            TimeLimit = original.TimeLimit;
        }
    }
}