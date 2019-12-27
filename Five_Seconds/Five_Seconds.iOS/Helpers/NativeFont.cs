using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Five_Seconds.Helpers;
using Five_Seconds.iOS.Helpers;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(NativeFont))]
namespace Five_Seconds.iOS.Helpers
{
    public class NativeFont : INativeFont
    {
        public float GetNativeSize(float size)
        {
            return size * (float)UIScreen.MainScreen.Scale;
        }
    }
}