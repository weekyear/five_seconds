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

        [OneToMany]
        public ObservableCollection<Record> Records { get; set; } = new ObservableCollection<Record>();

        [OneToOne]
        public Alarm Alarm { get; set; } = new Alarm();

        public Mission() { }

        public Mission (Mission original)
        {
            Id = original.Id;
            Name = original.Name;
            Alarm = original.Alarm;
            Percentage = original.Percentage;
        }
    }
}