using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("Records")]
    public class Record : IObject
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Alarm))]
        public int AlarmId { get; }
        
        public string Name { get; }

        public TimeSpan Time
        {
            get { return TimeOffset.LocalDateTime.TimeOfDay; }
        }

        public DateTime Date
        {
            get { return TimeOffset.LocalDateTime.Date; }
        }

        public DateTimeOffset TimeOffset { get; }

        public bool IsSuccess { get; set; }

        public Record(Alarm alarm, bool isSuccess)
        {
            AlarmId = alarm.Id;
            Name = alarm.Name;
            TimeOffset = alarm.TimeOffset;
            IsSuccess = isSuccess;
        }
    }
}
