using System;
using Xamarin.Forms;
using static Five_Seconds.ViewModels.RecordViewModel;

namespace Five_Seconds.CustomControls
{
    public class PercentLabel : Label
    {
        public static readonly BindableProperty PercentProperty =
            BindableProperty.Create(nameof(Percent),
                typeof(double),
                typeof(PercentLabel),
                -1.0,
                BindingMode.TwoWay,
                propertyChanged: OnPercentChanged);

        public double Percent
        {
            get
            {
                return (double)GetValue(PercentProperty);
            }
            set
            {
                SetValue(PercentProperty, value);
            }
        }

        public event EventHandler IsActiveChanged;

        static void OnPercentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var label = (PercentLabel)bindable;
            var successRate = label.Percent;

            if (successRate > 0.7)
            {
                label.TextColor = Color.DodgerBlue;
            }
            else if (successRate > 0.5)
            {
                label.TextColor = Color.Yellow;
            }
            else if (successRate == -1)
            {
                label.TextColor = Color.GhostWhite;
            }
            else
            {
                label.TextColor = Color.IndianRed;
            }

            //label.IsActiveChanged?.Invoke(label, null);
        }
    }
}
