using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class TypeButton : Button
    {
        public static readonly BindableProperty ButtonTypeProperty =
            BindableProperty.Create(nameof(ButtonType),
                typeof(int),
                typeof(TypeButton),
                0,
                BindingMode.TwoWay);

        public int ButtonType
        {
            get
            {
                return (int)GetValue(ButtonTypeProperty);
            }
            set
            {
                SetValue(ButtonTypeProperty, value);
            }
        }

        public static readonly BindableProperty AlarmTypeProperty =
            BindableProperty.Create(nameof(AlarmType),
                typeof(int),
                typeof(TypeButton),
                1,
                BindingMode.TwoWay,
                propertyChanged: OnAlarmTypeChanged);

        public int AlarmType
        {
            get
            {
                return (int)GetValue(AlarmTypeProperty);
            }
            set
            {
                SetValue(AlarmTypeProperty, value);
            }
        }

        static void OnAlarmTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var button = (TypeButton)bindable;

            int selectedType = (int)newValue;

            if (selectedType == button.ButtonType)
            {
                button.TextColor = Color.White;
                button.FontSize = Device.GetNamedSize(NamedSize.Medium, button);
                switch (button.ButtonType)
                {
                    case 0:
                        button.BackgroundColor = Color.FromHex("#64b5f6");
                        break;
                    case 1:
                        button.BackgroundColor = Color.FromHex("#3498DB");
                        break;
                    case 2:
                        button.BackgroundColor = Color.FromHex("#005cb2");
                        break;
                }
                button.FontAttributes = FontAttributes.Bold;
            }
            else
            {
                button.TextColor = Color.LightGray;
                button.FontSize = Device.GetNamedSize(NamedSize.Small, button);
                button.BackgroundColor = Color.White;
                button.FontAttributes = FontAttributes.None;
            }
        }
    }
}
