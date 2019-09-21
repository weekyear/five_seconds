using System;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class ListItemLabel : Label
    {
        public static readonly BindableProperty IsActiveProperty =
            BindableProperty.Create(nameof(IsActive),
                typeof(bool),
                typeof(ListItemLabel),
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

        public event EventHandler IsActiveChanged;

        static void OnIsActiveChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var label = (ListItemLabel)bindable;
            label.IsActiveChanged?.Invoke(label, null);
        }
    }
}
