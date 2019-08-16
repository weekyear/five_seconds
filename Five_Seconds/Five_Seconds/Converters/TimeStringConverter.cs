using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class TimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeValue = value as string;
            TimeSpan _time = DateTime.Parse(timeValue).TimeOfDay;
            return _time;
        }
        private object TimeToString(object value)
        {
            var timeSpan = (TimeSpan)value;

            if (timeSpan == TimeSpan.MinValue)
            {
                return "시간 없음";
            }
            var dateTime = new DateTime() + timeSpan;
            var timeString = string.Format(dateTime.ToShortTimeString());
            return timeString;
        }
    }
}
