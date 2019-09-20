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
    public partial class Alarm : INotifyPropertyChanged, IObject
    {
        public TimeSpan Time
        {
            get { return TimeOffset.LocalDateTime.TimeOfDay; }
            set { TimeOffset = GetDateTimeOffsetFromTimeSpan(value); }
        }
        public bool IsAlarmOn { get; set; } = true;
        public int Volume { get; set; } = 5;

        public bool OccursToday { get { return Days.Equals(DateTime.Now.DayOfWeek); } }
        public bool IsVibrateOn { get; set; } = true;
        public bool IsCountOn { get; set; } = true;

        //public int DaysId { get; set; }
        [OneToOne]
        public DaysOfWeek Days { get; set; } = new DaysOfWeek();
        public int DaysId { get; set; }
        public string Tone { get; set; } = AlarmTone.Tones[0].Name;

        public bool IsToday
        {
            get
            {
                if (Date.Subtract(DateTime.Now.Date).Days == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public DateTime Date
        {
            get { return TimeOffset.LocalDateTime.Date; }
            set { TimeOffset = GetDateTimeOffsetFromDateTime(value); }
        }

        public string DateString
        {
            get { return CreateDateString.CreateDateToString(this); }
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

            return new DateTimeOffset(dateTime);
        }

        public DateTime NextAlarmTime
        {
            get { return CalculateNextAlarmTime.NextAlarmTime(this); }
        }

        public Alarm(TimeSpan timeSpan)
        {
            Time = timeSpan;
        }
    }
}
