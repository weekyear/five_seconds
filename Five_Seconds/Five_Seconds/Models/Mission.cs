using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Five_Seconds.Models
{
    [Table("Missions")]
    public class Mission : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }
        public string Description { get; set; }
        public TimeSpan TimeOfDay { get; set; }
        public double Percentage { get; set; }
        public int TimeLimit { get; set; } = 5;

        [OneToMany]
        public ObservableCollection<Record> Records { get; set; } = new ObservableCollection<Record>();

        public Mission() { }

        public Mission (Mission original)
        {
            Id = original.Id;
            Description = original.Description;
            TimeOfDay = original.TimeOfDay;
            Percentage = original.Percentage;
            TimeLimit = original.TimeLimit;
        }
    }
}