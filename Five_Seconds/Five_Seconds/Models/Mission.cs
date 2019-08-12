using SQLite;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Five_Seconds.Models
{
    public class Mission : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, Unique]
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public string Percentage { get; set; }
        public int TimeLimit { get; set; }
        //public ObservableCollection<Record> Records { get; set; } = new ObservableCollection<Record>();

        public enum Type { Calculate, None };
    }
}