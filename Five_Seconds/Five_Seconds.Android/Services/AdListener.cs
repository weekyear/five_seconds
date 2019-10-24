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
using Five_Seconds.Droid.Interface;

namespace Five_Seconds.Droid.Services
{
    public class AdListener : Android.Gms.Ads.AdListener
    {
        private readonly IAdListener that;

        public AdListener(IAdListener t)
        {
            that = t;
        }

        public override void OnAdLoaded()
        {
            that.AdView.Visibility = ViewStates.Visible;
            base.OnAdLoaded();
        }

        public override void OnAdFailedToLoad(int errorCode)
        {
            that.AdView.Visibility = ViewStates.Gone;
            base.OnAdFailedToLoad(errorCode);
        }
    }
}