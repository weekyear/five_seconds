using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ScrollableNumberPicker), typeof(ScrollableNumberPickerRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class ScrollableNumberPickerRenderer : ViewRenderer<ScrollableNumberPicker, NumberPicker>
    {
        private Context _context;
        public ScrollableNumberPickerRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ScrollableNumberPicker> e)
        {
            base.OnElementChanged(e);

            var numberPicker = new NumberPicker(_context);

            switch (Element.PickerType)
            {
                case "AmPm":
                    numberPicker.MaxValue = 1;
                    numberPicker.MinValue = 0;
                    var pickerVals = new string[] { "오전", "오후" };
                    break;
                case "Hours":
                    numberPicker.MaxValue = 12;
                    numberPicker.MinValue = 1;
                    break;
                case "Minutes":
                    numberPicker.MaxValue = 59;
                    numberPicker.MinValue = 00;
                    break;
            }

            numberPicker.ValueChanged += NumberPicker_ValueChanged;

            SetNativeControl(numberPicker);
        }

        private void NumberPicker_ValueChanged(object sender, NumberPicker.ValueChangeEventArgs e)
        {
            Element.Value = e.NewVal;
        }
    }
}