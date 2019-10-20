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

[assembly: ExportRenderer(typeof(RoundedFrame), typeof(RoundedFrameRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class RoundedFrameRenderer : VisualElementRenderer<Frame>
    {
        public RoundedFrameRenderer()
        {
            SetBackgroundResource(Resource.Drawable.alarms_viewcell_background);
        }
    }
}