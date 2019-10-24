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
using Five_Seconds.Droid.Interface;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>, IAdListener
    {
        AdView _adView;
        public AdMobViewRenderer(Context context) : base(context) { }

        public AdView AdView => _adView;

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
            _adView = new AdView(Context)
            {
                AdSize = AdSize.Banner,
                AdUnitId = Element.AdUnitId
            };

            _adView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            //var requestbuilder = new AdRequest.Builder().AddTestDevice("FA3E0133F649B126EB4B86A6DA3E60D2").Build();
            //adView.LoadAd(requestbuilder);
            _adView.AdListener = new Services.AdListener(this);
            _adView.LoadAd(new AdRequest.Builder().Build());

            return _adView;
        }
    }
}