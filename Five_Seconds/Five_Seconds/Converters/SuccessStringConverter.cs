using Five_Seconds.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class SuccessStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IsSuccessToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == AppResources.Success)
            {
                return true;
            }
            else if ((string)value == AppResources.Failure)
            {
                return false;
            }
            return null;
        }
        private object IsSuccessToString(object value)
        {
            if ((bool)value)
            {
                return AppResources.Success;
            }
            else
            {
                return AppResources.Failure;
            }
        }
    }
}
