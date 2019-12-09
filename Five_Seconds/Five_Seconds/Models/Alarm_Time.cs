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
        public int Volume { get; set; } = 6;

        public bool OccursToday { get { return Days.Equals(DateTime.Now.DayOfWeek); } }
        public bool IsVibrateOn { get; set; } = true;

        public bool IsLaterAlarm
        {
            get 
            {
                bool isLaterAlarm = LaterAlarmTime != DateTime.MinValue;
                return isLaterAlarm;
            }
            set
            {
                if (value == false)
                {
                    LaterAlarmTime = DateTime.MinValue;
                }
            }
        }

        public DateTime LaterAlarmTime { get; set; } = DateTimeOffset.MinValue.UtcDateTime;

        [OneToOne]
        public DaysOfWeek Days { get; set; } = new DaysOfWeek();

        public bool HasADayBeenSelected
        {
            get { return DaysOfWeek.GetHasADayBeenSelected(Days); }
        }

        public int DaysId { get; set; }

        public string Tone { get; set; } = "Buzz";

        public DateTime Date
        {
            get { return TimeOffset.LocalDateTime.Date; }
            set { TimeOffset = GetDateTimeOffsetFromDateTime(value); }
        }

        public string DateString
        {
            get { return CreateDateString.CreateDateToString(this); }
        }

        private static DateTime DateTimeNow
        {
            get { return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0); }
        }

        public DateTimeOffset TimeOffset { get; set; } = new DateTimeOffset(DateTimeNow.AddMinutes(1));
        protected DateTimeOffset GetDateTimeOffsetFromTimeSpan(TimeSpan time)
        {
            var dateTime = new DateTime(Date.Year, Date.Month, Date.Day, time.Hours, time.Minutes, 0);
            return new DateTimeOffset(dateTime);
        }

        protected DateTimeOffset GetDateTimeOffsetFromDateTime(DateTime date)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, Time.Hours, Time.Minutes, 0);

            if (dateTime.Year > 9000) return DateTimeOffset.MaxValue;

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
