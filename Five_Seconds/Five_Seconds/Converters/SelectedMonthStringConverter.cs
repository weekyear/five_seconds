using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class SelectedMonthStringConverter : IValueConverter
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
            var dateTime = (DateTime)value;

            if (dateTime == DateTime.MinValue)
            {
                return "없음";
            }


            var monthString = $"{dateTime.Year}년 {dateTime.Month}월";
            return monthString;
        }
    }
}
