using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class AppIconImage : Image
    {
        public static readonly BindableProperty PackageNameProperty =
               BindableProperty.Create(nameof(PackageName),
                   typeof(string),
                   typeof(AppIconImage),
                   string.Empty,
                   BindingMode.TwoWay);

        public string PackageName
        {
            get
            {
                return (string)GetValue(PackageNameProperty);
            }
            set
            {
                SetValue(PackageNameProperty, value);
            }
        }
    }
}
