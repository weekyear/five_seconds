using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class PercentStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return PercentToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strPercent = value as string;
            if (strPercent.EndsWith("%"))
            {
                var num = decimal.Parse(strPercent.TrimEnd(new char[] { '%', ' ' })) / 100M;
                return num;
            }
            return null;
        }
        private object PercentToString(object value)
        {
            var strPercent = $"{((double)value * 100).ToString()}%";
            return strPercent;
        }
    }
}
