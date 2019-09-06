using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Helpers
{
    public class CalculateNextAlarmTime
    {
        public static DateTime NextAlarmTime(Alarm alarm)
        {
            var nextDate = CalculateNextDate(alarm);
            var nextTime = alarm.Time;

            var nextAlarmDateTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, nextTime.Hours, nextTime.Minutes, nextTime.Seconds);

            return nextAlarmDateTime;
        }

        private static DateTime CalculateNextDate(Alarm alarm)
        {
            if (DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                return DateTime.Now.Date.AddDays(CalculateAddingDaysWhenHasDaysOfWeek(alarm));
            }
            else
            {
                return alarm.Date;
            }
        }

        public static double CalculateAddingDaysWhenHasDaysOfWeek(Alarm alarm)
        {
            var allDays = alarm.Days.AllDays;

            int addingDays = 8;

            for (int i = 0; i < 7; i++)
            {
                if (allDays[i])
                {
                    var today = (int)DateTime.Now.DayOfWeek;
                    var diffDays = i - today > 0 ? i - today : i - today + 7;
                    if (addingDays > diffDays)
                    {
                        addingDays = diffDays;
                    }
                }
            }

            if (addingDays == 7 && alarm.Time.Subtract(DateTime.Now.TimeOfDay).Ticks > 0)
            {
                return 0;
            }

            return addingDays;
        }
    }
}
