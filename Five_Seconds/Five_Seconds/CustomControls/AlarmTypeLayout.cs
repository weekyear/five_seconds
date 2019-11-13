using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class AlarmTypeLayout : StackLayout
    {

        public static readonly BindableProperty LayoutTypeProperty =
               BindableProperty.Create(nameof(LayoutType),
                   typeof(int),
                   typeof(AlarmTypeLayout),
                   0,
                   BindingMode.TwoWay);

        public int LayoutType
        {
            get
            {
                return (int)GetValue(LayoutTypeProperty);
            }
            set
            {
                SetValue(LayoutTypeProperty, value);
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
            var viewCell = (AlarmTypeLayout)bindable;

            int selectedType = (int)newValue;

            switch (selectedType)
            {
                case 0:
                    SetIsVisible(selectedType, viewCell);
                    break;
                case 1:
                    SetIsVisible(selectedType, viewCell);
                    break;
                case 2:
                    SetIsVisible(selectedType, viewCell);
                    break;
            }
        }

        private static void SetIsVisible(int selectedType, AlarmTypeLayout viewCell)
        {
            if (selectedType == viewCell.LayoutType)
            {
                viewCell.IsVisible = true;
            }
            else
            {
                viewCell.IsVisible = false;
            }
        }

        private static void SetIsVisibleTypeTwo(AlarmTypeLayout viewCell)
        {
            switch (viewCell.LayoutType)
            {
                case 0:
                    viewCell.IsVisible = false;
                    break;
                case 1:
                    viewCell.IsVisible = true;
                    break;
                case 2:
                    viewCell.IsVisible = true;
                    break;
            }
        }
    }
}
