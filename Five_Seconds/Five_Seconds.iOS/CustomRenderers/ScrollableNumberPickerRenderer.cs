using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Five_Seconds.CustomControls;
using Five_Seconds.iOS.CustomRenderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ScrollableNumberPicker), typeof(ScrollableNumberPickerRenderer))]
namespace Five_Seconds.iOS.CustomRenderers
{
    public class ScrollableNumberPickerRenderer : ViewRenderer<ScrollableNumberPicker, UIPickerView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ScrollableNumberPicker> e)
        {
            base.OnElementChanged(e);

            var numberPicker = new UIPickerView();

            SetNativeControl(numberPicker);
        }
    }
}