using System;
using Android.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using SQLite;
using Five_Seconds.Repository;
using static Android.App.ActivityManager;

[assembly: Xamarin.Forms.Dependency(typeof(HelperAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class HelperAndroid : IHelper
    {
        public void CollapseSearchView()
        {
            var toolBar = GetToolbar();

            toolBar?.CollapseActionView();
        }

        Toolbar GetToolbar() => CrossCurrentActivity.Current.Activity.FindViewById<Toolbar>(Resource.Id.toolbar);


        public static bool IsApplicationInTheBackground()
        {
            bool isInBackground;

            RunningAppProcessInfo myProcess = new RunningAppProcessInfo();
            GetMyMemoryState(myProcess);
            isInBackground = myProcess.Importance != Importance.Foreground;

            return isInBackground;
        }
    }
}