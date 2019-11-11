using Five_Seconds.Resources;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("DaysOfWeek")]
    public class DaysOfWeek : IObject
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }
        public bool Sunday { get; set; } = false;
        public bool Monday { get; set; } = false;
        public bool Tuesday { get; set; } = false;
        public bool Wednesday { get; set; } = false;
        public bool Thursday { get; set; } = false;
        public bool Friday { get; set; } = false;
        public bool Saturday { get; set; } = false;

        public bool[] AllDays => new bool[] { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };
        public static string[] AllDaysString => new string[] { AppResources.Sunday, AppResources.Monday, AppResources.Tuesday, AppResources.Wednesday, AppResources.Thursday, AppResources.Friday, AppResources.Saturday };

        public DaysOfWeek() { }

        public DaysOfWeek(bool[] allDays)
        {
            if (allDays.Length != 7) return;

            Sunday = allDays[0];
            Monday = allDays[1];
            Tuesday = allDays[2];
            Wednesday = allDays[3];
            Thursday = allDays[4];
            Friday = allDays[5];
            Saturday = allDays[6];
        }

        public DaysOfWeek(bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday) : this(new bool[] { monday, tuesday, wednesday, thursday, friday, saturday, sunday })
        {
        }

        public DaysOfWeek(DaysOfWeek original)
        {
            Id = original.Id;
            Sunday = original.Sunday;
            Monday = original.Monday;
            Tuesday = original.Tuesday;
            Wednesday = original.Wednesday;
            Thursday = original.Thursday;
            Friday = original.Friday;
            Saturday = original.Saturday;
        }


        public static bool GetHasADayBeenSelected(DaysOfWeek days)
        {
            if (days == null) return false;
            return days.AllDays.Contains(true);
        }
    }
}
