using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class RecordStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return RecordToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strPercent = value as string;
            if (strPercent.EndsWith("초"))
            {
                var num = decimal.Parse(strPercent.TrimEnd(new char[] { '초', ' ' }));
                return num;
            }
            return null;
        }
        private object RecordToString(object value)
        {
            var strPercent = $"{((int)value).ToString()}초";
            return strPercent;
        }
    }
}
