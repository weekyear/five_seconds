//using Five_Seconds.Models;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Text;
//using Xamarin.Forms;

//namespace Five_Seconds.Converters
//{
//    public class BooleanCountSoundSwitchConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            return BooleanToColor(value);
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            return null;
//        }
//        private object BooleanToColor(object value)
//        {
//            var alarm = value as Alarm;
//            if (alarm.IsCountOn && alarm.IsCountSoundOn)
//            {
//                return Color.SkyBlue;
//            }
//            else
//            {
//                return Color.LightGray;
//            }
//        }
//    }
//}
