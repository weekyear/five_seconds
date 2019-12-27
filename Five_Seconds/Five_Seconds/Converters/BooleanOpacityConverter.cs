using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class BooleanOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BooleanToDouble(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _value = (double)value;
            if (_value == 1)
            {
                return true;
            }
            else if (_value == 0.5f)
            {
                return false;
            }
            return null;
        }
        private object BooleanToDouble(object value)
        {
            var isActive = (bool)value;
            if (isActive)
            {
                return 1;
            }
            else
            {
                return 0.35f;
            }
        }
    }
}
