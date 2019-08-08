using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
            return DateTime.Parse(value.ToString());
        }
        private object DateToString(object value)
        {
            if ((DateTime)value == DateTime.MinValue)
            {
                return "없음";
            }
            var dateTime = string.Format(((DateTime)value).ToShortDateString());
            return dateTime;
        }
    }
}
