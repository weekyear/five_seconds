using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class RoundedViewCell : ViewCell
    {
        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create("IsSelected", typeof(bool), typeof(RoundedViewCell), defaultValue: false);

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
    }
}
