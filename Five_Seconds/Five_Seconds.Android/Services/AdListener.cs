using Android.Views;
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
            base.OnAdLoaded();
        }

        public override void OnAdFailedToLoad(int errorCode)
        {
            that.AdView.Visibility = ViewStates.Gone;
            base.OnAdFailedToLoad(errorCode);
        }
    }
}