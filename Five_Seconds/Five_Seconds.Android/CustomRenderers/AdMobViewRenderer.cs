using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        public AdMobViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
                SetNativeControl(CreateAdView());
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(AdView.AdUnitId))
                Control.AdUnitId = Element.AdUnitId;
        }

        private AdView CreateAdView()
        {
            var adView = new AdView(Context)
            {
                AdSize = AdSize.Banner,
                AdUnitId = Element.AdUnitId
            };

            adView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            var requestbuilder = new AdRequest.Builder().AddTestDevice("FA3E0133F649B126EB4B86A6DA3E60D2").Build();
            //adView.AdListener = new AdListener(this);
            adView.LoadAd(requestbuilder);
            var mainActivity = CrossCurrentActivity.Current.Activity as MainActivity;
            adView.AdListener = new AdListener(mainActivity);
            //adView.LoadAd(new AdRequest.Builder().Build());

            return adView;
        }
        private class AdListener : Android.Gms.Ads.AdListener
        {
            private readonly MainActivity that;

            public AdListener(MainActivity t)
            {
                that = t;
            }

            public override void OnAdLoaded()
            {
                base.OnAdLoaded();
            }

            public override void OnAdFailedToLoad(int errorCode)
            {
                base.OnAdFailedToLoad(errorCode);
            }
        }
    }
}