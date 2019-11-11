using Five_Seconds.Models;
using Five_Seconds.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Five_Seconds.Helpers
{
    public class CreateDateString
    {
        public static string CreateDateToString(Alarm alarm)
        {
            if (alarm.IsLaterAlarm)
            {
                return AppResources.RingingSoon;
            }

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

            if (stringBuilder.ToString() == $"{AppResources.Sunday}, {AppResources.Monday}, {AppResources.Tuesday}, {AppResources.Wednesday}, {AppResources.Thursday}, {AppResources.Friday}, {AppResources.Saturday}")
            {
                return AppResources.Everyday;
            }
            else if (stringBuilder.ToString() == $"{AppResources.Sunday}, {AppResources.Saturday}")
            {
                return AppResources.Weekend;
            }
            else if (stringBuilder.ToString() == $"{AppResources.Monday}, {AppResources.Tuesday}, {AppResources.Wednesday}, {AppResources.Thursday}, {AppResources.Friday}")
            {
                return AppResources.Weekday;
            }
            else
            {
                return stringBuilder.ToString();
            }
        }

        private static string ConvertDateToString(DateTime date)
        {
            var allDaysString = DaysOfWeek.AllDaysString;

            switch (CultureInfo.CurrentCulture.Name)
            {
                case "ko-KR":
                    if (date.Date.Subtract(DateTime.Now.Date).TotalDays == 1)
                    {
                        return $"{string.Format("내일-{0:MM}월 {0:dd}일", date)}, ({allDaysString[(int)date.DayOfWeek]})";
                    }

                    return $"{string.Format("{0:MM}월 {0:dd}일", date)}, ({allDaysString[(int)date.DayOfWeek]})";
                case "en-US":
                    return $"{date.ToString("m", CultureInfo.CurrentCulture)}, ({date.DayOfWeek})";
                default:
                    return $"{date.ToString("m", CultureInfo.CurrentCulture)}, ({date.DayOfWeek})";
            }
        }

        public static string CreateNextDateTimeString(Alarm alarm)
        {
            DateTime nextTime;

            if (alarm != null)
            {
                if (alarm.IsLaterAlarm)
                {
                    nextTime = alarm.LaterAlarmTime;
                }
                else
                {
                    nextTime = alarm.NextAlarmTime;
                }
                bool isPastTime = nextTime.Subtract(DateTime.Now).Ticks < 0;

                if (isPastTime) return AppResources.NoNextAlarm;
            }
            else
            {
                return AppResources.NoNextAlarm;
            }

            var diffDays = nextTime.Date.Subtract(DateTime.Now.Date);
            string dateTimeString;

            switch (CultureInfo.CurrentCulture.Name)
            {
                case "ko-KR":
                    dateTimeString = GetDateTimeStringKO_KR(nextTime, diffDays);
                    break;
                case "en-US":
                    dateTimeString = GetDateTimeStringEN_US(nextTime, diffDays);
                    break;
                default:
                    dateTimeString = GetDateTimeStringEN_US(nextTime, diffDays);
                    break;
            }

            return dateTimeString;
        }

        private static string GetDateTimeStringKO_KR(DateTime nextTime, TimeSpan diffDays)
        {
            switch (diffDays.Days)
            {
                case 0:
                    return $"오늘 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}";
                case 1:
                    return $"내일 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}";
                case 2:
                    return $"모레 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}";
                default:
                    return $"{diffDays.Days}일 후 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}";
            }
        }

        private static string GetDateTimeStringEN_US(DateTime nextTime, TimeSpan diffDays)
        {
            switch (diffDays.Days)
            {
                case 0:
                    return $"Today, {nextTime.ToShortTimeString()}";
                case 1:
                    return $"Tomorrow, {nextTime.ToShortTimeString()}";
                default:
                    return $"Alarm in {diffDays.Days} days";
            }
        }

        public static string CreateTimeRemainingString(DateTime dateTime)
        {
            var diff = dateTime.Subtract(DateTime.Now);

            switch (CultureInfo.CurrentCulture.Name)
            {
                case "ko-KR":
                    return CreateTimeRemainingString_ko_KR(dateTime, diff);
                case "en-US":
                    return CreateTimeRemainingString_en_US(dateTime, diff);
                default:
                    return CreateTimeRemainingString_en_US(dateTime, diff);
            }
        }

        private static string CreateTimeRemainingString_ko_KR(DateTime dateTime, TimeSpan diff)
        {
            if (diff.Days > 0)
            {
                return $"{dateTime.Month}월 {dateTime.Day}일 {dateTime.ToString("tt")} {dateTime.Hour}:{dateTime.ToString("mm")}에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Hours > 0)
            {
                return $"{diff.Hours}시간 {diff.Minutes + 1}분 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Minutes > 0)
            {
                return $"{diff.Minutes + 1}분 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Seconds > 0)
            {
                return $"{diff.Seconds}초 후에 5초의 법칙을 실행합니다!";
            }
            else
            {
                return "이미 지난 시간입니다.";
            }
        }
        private static string CreateTimeRemainingString_en_US(DateTime dateTime, TimeSpan diff)
        {
            if (diff.Days > 0)
            {
                return $"Alarm set for  {dateTime.Hour}:{dateTime.ToString("mm")} {dateTime.ToString("tt")} on {dateTime.DayOfWeek}, {dateTime.Month} {dateTime.Day}";
            }
            else if (diff.Hours > 0)
            {
                return $"Alarm set for {diff.Hours} hours {diff.Minutes + 1} minutes from now.";
            }
            else if (diff.Minutes > 0)
            {
                return $"Alarm set for {diff.Minutes + 1} minutes from now.";
            }
            else if (diff.Seconds > 0)
            {
                return $"Alarm set for {diff.Seconds} seconds from now.";
            }
            else
            {
                return "이미 지난 시간입니다.";
            }
        }
    }
}
