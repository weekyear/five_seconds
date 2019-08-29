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


        public static bool GetHasADayBeenSelected(DaysOfWeek days)
        {
            if (days == null) return false;
            return days.AllDays.Contains(true);
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj is DayOfWeek)
        //    {
        //        //cast enum to int (sunday = 0, Saturday = 6)
        //        var dayOfWeek = (int)obj;
        //        if (dayOfWeek == 0)
        //        {
        //            if (Sunday)
        //                return true;
        //            else
        //                return false;
        //        }
        //        else
        //        {
        //            var day = AllDays[dayOfWeek - 1];
        //            if (day)
        //                return true;
        //            else
        //                return false;
        //        }
        //    }

        //    if (obj is DaysOfWeek)
        //    {
        //        var daysOfWeek = (DaysOfWeek)obj;
        //        if (this.AllDays == daysOfWeek.AllDays)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }

        //    return false;
        //}
    }
}
