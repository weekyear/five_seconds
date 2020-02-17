using Android.Gms.Ads;
using Five_Seconds.Droid.Services;

namespace Five_Seconds.Services
{
    public class InterstitialAdListener : Android.Gms.Ads.AdListener
    {
        readonly AdMobInterstitialAndroid _interstitialAd;

        public InterstitialAdListener(AdMobInterstitialAndroid interstitialAd)
        {
            _interstitialAd = interstitialAd;
        }

        public override void OnAdClosed()
        {
            _interstitialAd.LoadAd();
        }
    }
}
