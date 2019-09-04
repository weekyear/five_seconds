using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class ColoredTableView : TableView
    {
        public static BindableProperty GroupHeaderColorProperty = BindableProperty.Create("GroupHeaderColor", typeof(Color), typeof(ColoredTableView), Color.White);
        public Color GroupHeaderColor
        {
            get
            {
                return (Color)GetValue(GroupHeaderColorProperty);
            }
            set
            {
                SetValue(GroupHeaderColorProperty, value);
            }
        }
    }
}
