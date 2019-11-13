using System;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class SuccessLabel : Label
    {
        public static readonly BindableProperty IsSuccessProperty =
            BindableProperty.Create(nameof(IsSuccess),
                typeof(bool),
                typeof(SuccessLabel),
                false,
                BindingMode.TwoWay,
                propertyChanged: OnIsSuccessChanged);

        public bool IsSuccess
        {
            get
            {
                return (bool)GetValue(IsSuccessProperty);
            }
            set
            {
                SetValue(IsSuccessProperty, value);
            }
        }

        static void OnIsSuccessChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var label = (SuccessLabel)bindable;

            if (label.IsSuccess)
            {
                label.TextColor = Color.DarkBlue;
            }
            else
            {
                label.TextColor = Color.DarkRed;
            }
        }
    }
}
