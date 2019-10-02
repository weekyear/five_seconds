using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class SelectedWeekStringConverter : IValueConverter
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
            var startDate = (DateTime)value;

            if (startDate == DateTime.MinValue)
            {
                return "없음";
            }

            var endDate = startDate.AddDays(6);

            var dateOfWeekString = $"{startDate.Month}.{startDate.Day} ~ {endDate.Month}.{endDate.Day}";
            return dateOfWeekString;
        }
    }
}
