using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AppIconImage), typeof(AppIconImageRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class AppIconImageRenderer : ImageRenderer
    {
        public AppIconImageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            var appIconImage = Element as AppIconImage;
            SetImageDrawable(appIconImage);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == AppIconImage.PackageNameProperty.PropertyName)
            {
                try
                {
                    var appIconImage = Element as AppIconImage;
                    SetImageDrawable(appIconImage);
                }
                catch (ObjectDisposedException error)
                {
                    Console.WriteLine(error.Message + "_RoundedViewCellRenderer");
                }
            }
        }

        private void SetImageDrawable(AppIconImage appIconImage)
        {
            if (!string.IsNullOrEmpty(appIconImage.PackageName))
            {
                Drawable icon = Context.PackageManager.GetApplicationIcon(appIconImage.PackageName);
                Control.SetImageDrawable(icon);
            }
            else
            {
                Drawable icon = Context.GetDrawable(Resource.Drawable.ic_blank_app);
                Control.SetImageDrawable(icon);
            }
        }
    }
}