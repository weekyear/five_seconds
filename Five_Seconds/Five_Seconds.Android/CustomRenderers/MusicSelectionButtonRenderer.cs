using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Button = Xamarin.Forms.Button;
using ButtonRenderer = Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer;

[assembly: ExportRenderer(typeof(MusicSelectionButton), typeof(MusicSelectionButtonRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class MusicSelectionButtonRenderer : ButtonRenderer
    {
        public MusicSelectionButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // if the text color has not been overridden, display this default
                if (Element.TextColor == (Color)Button.TextColorProperty.DefaultValue)
                {
                    Control.SetTextColor(Xamarin.Forms.Color.FromHex("#B1C3FF").ToAndroid());
                }
                Control.SetAllCaps(false);

                // set background and drawables
                //Control.SetCompoundDrawablesRelativeWithIntrinsicBounds(ResourcesCompat.GetDrawable(Resources, Resource.Drawable.ic_plus, null), null, null, null);
                Control.SetPaddingRelative(PaddingStart + 80, PaddingTop, PaddingEnd, PaddingBottom);

                // remove state list shadow
                Control.StateListAnimator = null;
            }
        }
    }
}