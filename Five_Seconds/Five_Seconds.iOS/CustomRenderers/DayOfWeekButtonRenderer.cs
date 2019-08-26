using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Five_Seconds.CustomControls;
using Five_Seconds.iOS.CustomRenderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DayOfWeekButton), typeof(DayOfWeekButtonRenderer))]
namespace Five_Seconds.iOS.CustomRenderers
{
    public class DayOfWeekButtonRenderer : ButtonRenderer
    {
        DayOfWeekButton _dowButton;
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                _dowButton = (DayOfWeekButton)Element;
                _dowButton.Clicked += Button_Clicked;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            _dowButton.IsSelected = !_dowButton.IsSelected;
        }

        protected override void Dispose(bool disposing)
        {
            _dowButton.Clicked -= Button_Clicked;
            base.Dispose(disposing);
        }
    }
}