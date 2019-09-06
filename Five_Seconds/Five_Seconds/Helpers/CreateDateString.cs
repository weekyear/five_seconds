using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Helpers
{
    public class CreateDateString
    {
        public static string DateToString(Alarm alarm)
        {
            if (DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                return ConvertDaysOfWeekToString(alarm);
            }

            return ConvertDateToString(alarm.Date);
        }

        private static string ConvertDaysOfWeekToString(Alarm alarm)
        {
            var stringBuilder = new StringBuilder();

            var allDays = alarm.Days.AllDays;
            var allDaysString = DaysOfWeek.AllDaysString;

            for (int i = 0; i < 7; i++)
            {
                if (allDays[i])
                {
                    stringBuilder.Append($", {allDaysString[i]}");
                }
            }
            stringBuilder.Remove(0, 2);

            return stringBuilder.ToString();
        }

        private static string ConvertDateToString(DateTime date)
        {
            var allDaysString = DaysOfWeek.AllDaysString;

            if (date.Date.Subtract(DateTime.Now.Date).TotalDays == 1)
            {
                return $"내일-{string.Format("{0:MM}월 {0:dd}일", date)}, ({allDaysString[(int)date.DayOfWeek]})";
            }

            var dateTime = $"{string.Format("{0:MM}월 {0:dd}일", date)}, ({allDaysString[(int)date.DayOfWeek]})";
            return dateTime;
        }
    }
}
