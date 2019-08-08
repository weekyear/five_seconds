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
            return DateTime.Parse(value.ToString());
        }
        private object TimeToString(object value)
        {
            if ((DateTime)value == DateTime.MinValue)
            {
                return "시간 없음";
            }
            var dateTime = string.Format(((DateTime)value).ToShortTimeString());
            return dateTime;
        }
    }
}
