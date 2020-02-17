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

            var nextAlarmDateTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, nextTime.Hours, nextTime.Minutes, 0);

            return nextAlarmDateTime;
        }

        private static DateTime CalculateNextDate(Alarm alarm)
        {
            if (DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                //return DateTime.Now.Date.AddDays(CalculateAddingDaysWhenHasDaysOfWeek(alarm));
                if (!alarm.IsGoOffPreAlarm)
                {
                    return DateTime.Now.Date.AddDays(CalculateAddingDaysWhenHasDaysOfWeek(alarm));
                }
                else
                {
                    return DateTime.Now.Date.AddDays(CalculateAddingDaysWhenHasDaysOfWeekAndGoOffPreAlarm(alarm));
                }
            }
            else
            {
                if (alarm.Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0 && alarm.Date == DateTime.Now.Date)
                {
                    return alarm.Date.AddDays(1);
                }
                else
                {
                    return alarm.Date;
                }
            }
        }

        private static double CalculateAddingDaysWhenHasDaysOfWeek(Alarm alarm)
        {
            var allDays = alarm.Days.AllDays;

            int addingDays = 8;
            int diffDays;

            bool isPastTime = alarm.Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0;

            for (int i = 0; i < 7; i++)
            {
                if (allDays[i])
                {
                    var today = (int)DateTime.Now.DayOfWeek;

                    if (isPastTime)
                    {
                        diffDays = i - today > 0 ? i - today : i - today + 7;
                    }
                    else
                    {
                        diffDays = i - today >= 0 ? i - today : i - today + 7;
                    }
                        
                    if (addingDays > diffDays)
                    {
                        addingDays = diffDays;
                    }
                }
            }

            return addingDays;
        }

        private static double CalculateAddingDaysWhenHasDaysOfWeekAndGoOffPreAlarm(Alarm alarm)
        {
            var allDays = alarm.Days.AllDays;

            int addingDays = 8;
            int diffDays;

            for (int i = 0; i < 7; i++)
            {
                if (allDays[i])
                {
                    var today = (int)DateTime.Now.DayOfWeek;

                    diffDays = i - today > 0 ? i - today : i - today + 7;

                    if (addingDays > diffDays)
                    {
                        addingDays = diffDays;
                    }
                }
            }

            return addingDays;
        }

        public static DateTime NextAlarmTimeExceptForPreAlarm(Alarm alarm)
        {
            var nextDate = CalculateNextDateExceptForPreAlarm(alarm);
            var nextTime = alarm.Time;

            var nextAlarmDateTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, nextTime.Hours, nextTime.Minutes, 0);

            return nextAlarmDateTime;
        }

        private static DateTime CalculateNextDateExceptForPreAlarm(Alarm alarm)
        {
            if (DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                return DateTime.Now.Date.AddDays(CalculateAddingDaysWhenHasDaysOfWeekAndGoOffPreAlarm(alarm));
            }
            else
            {
                if (alarm.Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0 && alarm.Date == DateTime.Now.Date)
                {
                    return alarm.Date.AddDays(1);
                }
                else
                {
                    return alarm.Date;
                }
            }
        }
    }
}
