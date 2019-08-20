using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("Alarms")]
    public class Alarm : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset TimeOffset { get; set; }

        public TimeSpan Time
        {
            get { return TimeOffset.LocalDateTime.TimeOfDay; }
            set { TimeOffset = GetDateTimeOffsetFromTimeSpan(value); }
        }
        protected DateTimeOffset GetDateTimeOffsetFromTimeSpan(TimeSpan time)
        {
            var now = DateTime.Now;
            var dateTime = new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, time.Seconds);
            return new DateTimeOffset(dateTime);
        }
    }
}
