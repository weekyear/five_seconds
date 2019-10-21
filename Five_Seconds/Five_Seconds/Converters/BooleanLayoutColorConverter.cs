﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class BooleanLayoutColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BooleanToColor(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            if (color == Color.LightSkyBlue)
            {
                return true;
            }
            else if (color == Color.White)
            {
                return false;
            }
            return null;
        }
        private object BooleanToColor(object value)
        {
            var isSelected = (bool)value;
            if (isSelected)
            {
                return Color.LightSkyBlue;
            }
            else
            {
                return Color.White;
            }
        }
    }
}