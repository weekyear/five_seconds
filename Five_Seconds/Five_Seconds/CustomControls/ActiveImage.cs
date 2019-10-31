using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class ActiveImage : Image
    {
        public static readonly BindableProperty IsActiveProperty =
            BindableProperty.Create(nameof(IsActive),
                typeof(bool),
                typeof(ActiveImage),
                false,
                BindingMode.TwoWay,
                propertyChanged: OnIsActiveChanged);

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

        public static readonly BindableProperty ImageSourceActiveOnProperty =
            BindableProperty.Create(nameof(ImageSourceActive),
                typeof(string),
                typeof(ActiveImage),
                "");

        public string ImageSourceActive
        {
            get { return (string)GetValue(ImageSourceActiveOnProperty); }
            set { SetValue(ImageSourceActiveOnProperty, value); }
        }

        public static readonly BindableProperty ImageSourceInactiveProperty =
            BindableProperty.Create(nameof(ImageSourceInactive),
                typeof(string),
                typeof(ActiveImage),
                "");

        public string ImageSourceInactive
        {
            get { return (string)GetValue(ImageSourceInactiveProperty); }
            set { SetValue(ImageSourceInactiveProperty, value); }
        }

        static void OnIsActiveChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var image = (ActiveImage)bindable;

            if (image.IsActive)
            {
                image.Source = image.ImageSourceActive;
            }
            else
            {
                image.Source = image.ImageSourceInactive;
            }
        }
    }
}
