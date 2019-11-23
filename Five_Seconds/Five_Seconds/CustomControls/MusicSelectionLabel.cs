using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class MusicSelectionLabel : Label
    {
        public static readonly BindableProperty IsActiveProperty =
            BindableProperty.Create(nameof(IsActive),
                typeof(bool),
                typeof(MusicSelectionLabel),
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

        static void OnIsActiveChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var label = (MusicSelectionLabel)bindable;

            if (label.IsActive)
            {
                label.TextColor = Color.DarkBlue;
            }
            else
            {
                label.TextColor = Color.LightGray;
            }

            //label.IsActiveChanged?.Invoke(label, null);
        }
    }
}
