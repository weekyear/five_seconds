using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class BooleanSwitchColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BooleanToColor(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            if (color == Color.SkyBlue)
            {
                return true;
            }
            else if (color == Color.LightGray)
            {
                return false;
            }
            return null;
        }
        private object BooleanToColor(object value)
        {
            Color color;
            var isActive = (bool)value;
            if (isActive)
            {
                return Color.SkyBlue;
            }
            else
            {
                return Color.LightGray;
            }
        }
    }
}
