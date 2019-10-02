﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class DateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeValue = value as string;
            DateTime _time = DateTime.Parse(timeValue);
            return _time;
        }
        private object DateToString(object value)
        {
            var dateTime = (DateTime)value;

            if (dateTime == DateTime.MinValue)
            {
                return "날짜 없음";
            }
            var dateString = $"{dateTime.Month}월 {dateTime.Day}일";
            return dateString;
        }
    }
}
