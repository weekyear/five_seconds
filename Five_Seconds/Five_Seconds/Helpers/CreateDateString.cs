﻿using Five_Seconds.Models;
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
                return $"내일";
            }

            var dateTime = $"{string.Format("{0:MM}월 {0:dd}일", date)}, ({allDaysString[(int)date.DayOfWeek]})";
            return dateTime;
        }

        public static string CreateNextDateTimeString(Alarm alarm)
        {
            if (alarm == null) return "다음 울릴 알람이 없습니다.";

            var nextTime = alarm.NextAlarmTime;

            var diffDays = nextTime.Date.Subtract(DateTime.Now.Date);
            string dateTimeString;

            switch (diffDays.Days)
            {
                case 0:
                    dateTimeString = $"오늘 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}에 실행";
                    break;
                case 1:
                    dateTimeString = $"내일 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}에 실행";
                    break;
                case 2:
                    dateTimeString = $"모레 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}에 실행";
                    break;
                default:
                    dateTimeString = $"{diffDays.Days}일 후 {string.Format("{0:tt} {0:hh}시 {0:mm}분", nextTime)}에 실행";
                    break;

            }

            return dateTimeString;
        }
    }
}
