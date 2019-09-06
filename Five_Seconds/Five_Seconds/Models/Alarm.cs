using Five_Seconds.Helpers;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("Alarm")]
    public class Alarm : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }
        public TimeSpan Time
        {
            get { return TimeOffset.LocalDateTime.TimeOfDay; }
            set { TimeOffset = GetDateTimeOffsetFromTimeSpan(value); }
        }
        public bool IsAlarmOn { get; set; } = true;
        public int Volume { get; set; } = 5;

        public bool OccursToday { get { return Days.Equals(DateTime.Now.DayOfWeek); } }
        public bool IsVibrateOn { get; set; } = true;

        //public int DaysId { get; set; }
        [OneToOne]
        public DaysOfWeek Days { get; set; } = new DaysOfWeek();
        public string Tone { get; set; } = AlarmTone.Tones[0].Name;

        public bool IsToday { get; set; } = true;

        public DateTime Date
        {
            get { return TimeOffset.LocalDateTime.Date; }
            set { TimeOffset = GetDateTimeOffsetFromDateTime(value); }
        }

        public string DateString
        {
            get { return CreateDateString.DateToString(this); }
        }

        public DateTimeOffset TimeOffset { get; set; } = new DateTimeOffset(DateTime.Now);
        protected DateTimeOffset GetDateTimeOffsetFromTimeSpan(TimeSpan time)
        {
            var dateTime = new DateTime(Date.Year, Date.Month, Date.Day, time.Hours, time.Minutes, time.Seconds);
            return new DateTimeOffset(dateTime);
        }

        protected DateTimeOffset GetDateTimeOffsetFromDateTime(DateTime date)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, Time.Hours, Time.Minutes, Time.Seconds);

            if (date.Date == DateTime.Now.Date)
            {
                IsToday = true;
            }

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
            IsVibrateOn = original.IsVibrateOn;
            Days = original.Days;
            Tone = original.Tone;
        }
    }
}
