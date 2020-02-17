
using Android.App;
using Android.Gms.Ads;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;
using System.Diagnostics;

[assembly: Xamarin.Forms.Dependency(typeof(AdMobInterstitialAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class AdMobInterstitialAndroid : IAdMobInterstitial
    {
        public InterstitialAd _interstitialAd;

        public void Start()
        {
            var context = Application.Context;
            _interstitialAd = new InterstitialAd(context)
            {
                AdUnitId = "ca-app-pub-8413101784746060/6812351989"
            };

            var adListener = new InterstitialAdListener(this);

            _interstitialAd.AdListener = adListener;

            LoadAd();
        }

        public void LoadAd()
        {
            _interstitialAd.LoadAd(new AdRequest.Builder().Build());
            //CreateRequestBuilderWhenTest();
        }

        public void Show()
        {
            if (_interstitialAd.IsLoaded) _interstitialAd.Show();
            else LoadAd();
        }

        [Conditional("DEBUG")]
        private void CreateRequestBuilderWhenTest()
        {
            _interstitialAd.LoadAd(new AdRequest.Builder().AddTestDevice("FA3E0133F649B126EB4B86A6DA3E60D2").Build());
        }
    }
}