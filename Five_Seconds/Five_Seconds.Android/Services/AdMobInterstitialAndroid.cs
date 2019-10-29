
using Android.App;
using Android.Gms.Ads;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AdMobInterstitialAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class AdMobInterstitialAndroid : IAdMobInterstitial
    {
        InterstitialAd _ad;

        public void Show(string adUnit)
        {
            var context = Application.Context;
            _ad = new InterstitialAd(context)
            {
                AdUnitId = adUnit
            };

            var intlistener = new InterstitialAdListener(_ad);
            intlistener.OnAdLoaded();
            _ad.AdListener = intlistener;

            var requestbuilder = new AdRequest.Builder().AddTestDevice("FA3E0133F649B126EB4B86A6DA3E60D2").Build();

            //var requestbuilder = new AdRequest.Builder().Build();
            _ad.LoadAd(requestbuilder);
        }
    }
}