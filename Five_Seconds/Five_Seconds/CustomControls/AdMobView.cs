using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class AdMobView : View
    {
        public static readonly BindableProperty AdUnitIdProperty = BindableProperty.Create(
                      nameof(AdUnitId),
                      typeof(string),
                      typeof(AdMobView),
                      string.Empty);

        public static readonly BindableProperty AdSizeProperty = BindableProperty.Create(
                      nameof(AdSize),
                      typeof(string),
                      typeof(AdMobView),
                      string.Empty);

        public string AdUnitId
        {
            get => (string)GetValue(AdUnitIdProperty);
            set => SetValue(AdUnitIdProperty, value);
        }

        public string AdSize
        {
            get => (string)GetValue(AdSizeProperty);
            set => SetValue(AdSizeProperty, value);
        }
    }
}
