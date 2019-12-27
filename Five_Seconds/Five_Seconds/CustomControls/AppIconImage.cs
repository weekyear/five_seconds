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


        public static readonly BindableProperty IsActiveProperty =
            BindableProperty.Create(nameof(IsActive),
                typeof(bool),
                typeof(AppIconImage),
                false,
                BindingMode.TwoWay,
                propertyChanged: IsActiveChanged);

        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }

        static void IsActiveChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var image = (AppIconImage)bindable;

            if (image.IsActive)
            {
                image.Opacity = 1f;
            }
            else
            {
                image.Opacity = 0.3f;
            }
        }
    }
}
