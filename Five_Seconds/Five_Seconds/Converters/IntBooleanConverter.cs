using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class IntBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IntToBoolean(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BooleanToInt(value);
        }
        private object IntToBoolean(object value)
        {
            var intValue = (int)value;
            if (intValue == 0)
            {
                return false;
            }
            else if (intValue == 1)
            {
                return true;
            }
            return false;
        }

        private object BooleanToInt(object value)
        {
            var bollValue = (bool)value;
            if (bollValue)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
