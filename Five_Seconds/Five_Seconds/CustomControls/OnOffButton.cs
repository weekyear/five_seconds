using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class OnOffButton : Button
    {
        public static readonly BindableProperty IsActiveProperty =
            BindableProperty.Create("IsActive",
                typeof(bool),
                typeof(OnOffButton),
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

        public static readonly BindableProperty ImageSourceIsOnProperty =
            BindableProperty.Create("ImageSourceIsOn",
                typeof(string),
                typeof(OnOffButton),
                "");

        public string ImageSourceIsOn
        {
            get { return (string)GetValue(ImageSourceIsOnProperty); }
            set { SetValue(ImageSourceIsOnProperty, value); }
        }

        public static readonly BindableProperty ImageSourceIsOffProperty =
            BindableProperty.Create("ImageSourceIsOff",
                typeof(string),
                typeof(OnOffButton),
                "");

        public string ImageSourceIsOff
        {
            get { return (string)GetValue(ImageSourceIsOffProperty); }
            set { SetValue(ImageSourceIsOffProperty, value); }
        }

        public event EventHandler IsActiveChanged;

        static void OnIsActiveChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var button = (OnOffButton)bindable;
            button.IsActiveChanged?.Invoke(button, null);
        }
    }
}
