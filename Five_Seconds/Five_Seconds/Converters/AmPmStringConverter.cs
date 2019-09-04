using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class AmPmStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return AmPmToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeValue = value as string;
            TimeSpan _time = DateTime.Parse(timeValue).TimeOfDay;
            return _time;
        }
        private object AmPmToString(object value)
        {
            var timeSpan = (TimeSpan)value;

            if (timeSpan == TimeSpan.MinValue)
            {
                return "";
            }
            var dateTime = new DateTime() + timeSpan;
            var timeString = string.Format("{0:tt}", dateTime);
            return timeString;
        }
    }
}
