using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class AlarmTypeBoxView : BoxView
    {

        public static readonly BindableProperty ViewCellTypeProperty =
               BindableProperty.Create(nameof(ViewType),
                   typeof(int),
                   typeof(AlarmTypeBoxView),
                   0,
                   BindingMode.TwoWay);

        public int ViewType
        {
            get
            {
                return (int)GetValue(ViewCellTypeProperty);
            }
            set
            {
                SetValue(ViewCellTypeProperty, value);
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
            var viewCell = (AlarmTypeBoxView)bindable;

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
                    SetIsVisibleTypeTwo(viewCell);
                    break;
            }
        }

        private static void SetIsVisible(int selectedType, AlarmTypeBoxView viewCell)
        {
            if (selectedType == viewCell.ViewType)
            {
                viewCell.IsVisible = true;
            }
            else
            {
                viewCell.IsVisible = false;
            }
        }

        private static void SetIsVisibleTypeTwo(AlarmTypeBoxView viewCell)
        {
            switch (viewCell.ViewType)
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
