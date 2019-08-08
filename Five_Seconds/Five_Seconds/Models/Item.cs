using SQLite;
using System;
using System.Collections.ObjectModel;

namespace Five_Seconds.Models
{
    public class Mission
    {
        [PrimaryKey, AutoIncrement]
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public string Percentage { get; set; }
        public int TimeLimit { get; set; }
        public ObservableCollection<Record> Records { get; set; } = new ObservableCollection<Record>();
        public enum Type { Calculate, None };
    }
}