﻿using System;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class ScrollableNumberPicker : View
    {
        public ScrollableNumberPicker() { }

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value),
                typeof(int),
                typeof(ScrollableNumberPicker),
                0,
                BindingMode.TwoWay);

        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly BindableProperty PickerTypeProperty =
            BindableProperty.Create(nameof(PickerType),
                typeof(string),
                typeof(ScrollableNumberPicker),
                "null",
                BindingMode.TwoWay);

        public string PickerType
        {
            get
            {
                return (string)GetValue(PickerTypeProperty);
            }
            set
            {
                SetValue(PickerTypeProperty, value);
            }
        }
    }
}
