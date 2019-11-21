using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Converters
{
    public class AppLabelStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return AppLabelToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strPercent = value as string;
            return null;
        }
        private object AppLabelToString(object value)
        {
            string labelStr;
            if (!string.IsNullOrEmpty((string)value))
            {
                labelStr = $"{((string)value).ToString()} 앱과 연동";
            }
            else
            {
                labelStr = $"연동할 앱 설정";
            }
            return labelStr;
        }
    }
}
