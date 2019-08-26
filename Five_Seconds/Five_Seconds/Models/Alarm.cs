using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("Alarm")]
    public class Alarm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }

        public TimeSpan Time
        {
            get { return TimeOffset.LocalDateTime.TimeOfDay; }
            set { TimeOffset = GetDateTimeOffsetFromTimeSpan(value); }
        }
        public bool IsActive { get; set; }
        public bool IsAlarmOn { get; set; } = false;
        public int Volume { get; set; } = 5;

        public bool OccursToday { get { return Days.Equals(DateTime.Now.DayOfWeek); } }
        public bool IsVibrateOn { get; set; } = false;
        public int VibeFrequency { get; set; } = 5;

        [OneToOne]
        public DaysOfWeek Days { get; set; } = new DaysOfWeek();
        public string Tone { get; set; }


        public DateTimeOffset TimeOffset { get; set; } = new DateTimeOffset(DateTime.Now);
        protected DateTimeOffset GetDateTimeOffsetFromTimeSpan(TimeSpan time)
        {
            var now = DateTime.Now;
            var dateTime = new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, time.Seconds);
            return new DateTimeOffset(dateTime);
        }

        public Alarm()
        {
        }
        public Alarm(TimeSpan timeSpan)
        {
            Time = timeSpan;
        }

        public Alarm(Alarm original)
        {
            TimeOffset = original.TimeOffset;
            IsActive = original.IsActive;
            IsVibrateOn = original.IsVibrateOn;
            Days = original.Days;
            Tone = original.Tone;
        }
    }
}
