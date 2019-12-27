using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Helpers;
using Five_Seconds.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(NativeFont))]
namespace Five_Seconds.Droid.Helpers
{
    public class NativeFont : INativeFont
    {
        public float GetNativeSize(float size)
        {
            var displayMetrics = Android.App.Application.Context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, size, displayMetrics);
        }
    }
}