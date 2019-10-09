using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("Record")]
    public class Record : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }

        public int AlarmId { get; set; }
        
        public string Name { get; set; }

        public TimeSpan Time
        {
            get { return DateTime.TimeOfDay; }
        }

        public DateTime Date
        {
            get { return DateTime.Date; }
        }

        public DateTime DateTime { get; set; }

        public bool IsSuccess { get; set; }

        public Record() { }

        public Record(Alarm alarm, bool isSuccess)
        {
            AlarmId = alarm.Id;
            Name = alarm.Name;
            DateTime = DateTime.Now;
            IsSuccess = isSuccess;
        }
    }
}
