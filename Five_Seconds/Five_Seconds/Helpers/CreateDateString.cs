using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Helpers
{
    public class CreateDateString
    {
        public static string CreateDateToString(Alarm alarm)
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

            if (stringBuilder.ToString() == "일, 월, 화, 수, 목, 금, 토")
            {
                return "매일";
            }
            else if (stringBuilder.ToString() == "일, 토")
            {
                return "주말";
            }
            else if (stringBuilder.ToString() == "월, 화, 수, 목, 금")
            {
                return "평일";
            }
            else
            {
                return stringBuilder.ToString();
            }
        }

        private static string ConvertDateToString(DateTime date)
        {
            var allDaysString = DaysOfWeek.AllDaysString;

            if (date.Date.Subtract(DateTime.Now.Date).TotalDays == 1)
            {
                return $"{string.Format("내일-{0:MM}월 {0:dd}일", date)}, ({allDaysString[(int)date.DayOfWeek]})";
            }

            var dateTime = $"{string.Format("{0:MM}월 {0:dd}일", date)}, ({allDaysString[(int)date.DayOfWeek]})";
            return dateTime;
        }

        public static string CreateNextDateTimeString(Alarm alarm)
        {
            DateTime nextTime;

            if (alarm != null)
            {
                nextTime = alarm.NextAlarmTime;
                bool isPastTime = nextTime.Subtract(DateTime.Now).Ticks < 0;

                if (isPastTime) return "다음 알람이 없습니다.";
            }
            else
            {
                return "다음 알람이 없습니다.";
            }

            var diffDays = nextTime.Date.Subtract(DateTime.Now.Date);
            string dateTimeString;

            switch (diffDays.Days)
            {
                case 0:
                    dateTimeString = $"오늘 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}";
                    break;
                case 1:
                    dateTimeString = $"내일 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}";
                    break;
                case 2:
                    dateTimeString = $"모레 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}";
                    break;
                default:
                    dateTimeString = $"{diffDays.Days}일 후 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}";
                    break;

            }

            return dateTimeString;
        }

        public static string CreateTimeRemainingString(DateTime dateTime)
        {
            var diff = dateTime.Subtract(DateTime.Now);

            if (diff.Days > 0)
            {
                return $"{dateTime.Month}월 {dateTime.Day}일 {dateTime.ToString("tt")} {dateTime.Hour}:{dateTime.Minute}에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Hours > 0)
            {
                return $"{diff.Hours}시간 {diff.Minutes}분 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Minutes > 0)
            {
                return $"{diff.Minutes}분 후에 5초의 법칙을 실행합니다!";
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
    }
}
