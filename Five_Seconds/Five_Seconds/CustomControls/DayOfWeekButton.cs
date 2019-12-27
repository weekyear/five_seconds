using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class DayOfWeekButton : Button
    {
        public static readonly BindableProperty IsSelectedProperty = 
            BindableProperty.Create("IsSelected", 
                typeof(bool), 
                typeof(DayOfWeekButton), 
                false, 
                BindingMode.TwoWay,
                propertyChanged : IsSelectedPropertyChanged);

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        public event EventHandler IsSelectedChanged;

        static void IsSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var button = (DayOfWeekButton)bindable;
            button.IsSelectedChanged?.Invoke(button, null);
        }
    }
}
