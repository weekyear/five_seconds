using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class MissionDateStringConverter : IValueConverter
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
            var date = (DateTime)value;

            if (date.Subtract(DateTime.Now) == TimeSpan.FromDays(1))
            {
                return $"내일-{date.ToShortDateString()},({date.DayOfWeek})";
            }

            if (date == DateTime.MinValue)
            {
                return "없음";
            }
            var dateTime = $"{date.ToShortDateString()},({date.DayOfWeek})";
            return dateTime;
        }
    }
}
