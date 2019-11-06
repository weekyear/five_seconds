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
using Five_Seconds.Droid.Services;
using Five_Seconds.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(CrashTestAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class CrashTestAndroid : ICrashTest
    {
        public void CrashTest()
        {
            var instance = Crashlytics.Crashlytics.Instance;
            //instance.DebugMode = true;
            instance.Crash();
        }
    }
}