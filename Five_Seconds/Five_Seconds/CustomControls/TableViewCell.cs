using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class TableViewCell : ViewCell
    {
        public static readonly BindableProperty AllowHighlightProperty =
            BindableProperty.Create("AllowHighlight", typeof(bool), typeof(TableViewCell), defaultValue: true);

        public bool AllowHighlight
        {
            get { return (bool)GetValue(AllowHighlightProperty); }
            set { SetValue(AllowHighlightProperty, value); }
        }
    }
}
