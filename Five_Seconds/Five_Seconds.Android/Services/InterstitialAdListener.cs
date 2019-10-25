using Android.Gms.Ads;

namespace Five_Seconds.Services
{
    public class InterstitialAdListener : AdListener
    {
        readonly InterstitialAd _ad;

        public InterstitialAdListener(InterstitialAd ad)
        {
            _ad = ad;
        }

        public override void OnAdLoaded()
        {
            base.OnAdLoaded();

            if (_ad.IsLoaded)
                _ad.Show();
        }
    }
}
