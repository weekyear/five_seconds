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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using SearchView = Android.Support.V7.Widget.SearchView;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;
using Plugin.CurrentActivity;

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
    }
}