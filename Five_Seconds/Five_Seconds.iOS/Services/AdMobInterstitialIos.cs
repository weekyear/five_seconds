using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Five_Seconds.Services;
using Foundation;
using UIKit;
using Google.MobileAds;
using Five_Seconds.iOS.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AdMobInterstitialIos))]
namespace Five_Seconds.iOS.Services
{
    public class AdMobInterstitialIos : IAdMobInterstitial
    {
        Interstitial _adInterstitial;

        public void Show(string adUnit)
        {
            _adInterstitial = new Interstitial(adUnit);
            var request = Request.GetDefaultRequest();
            _adInterstitial.AdReceived += (sender, args) =>
            {
                if (_adInterstitial.IsReady)
                {
                    var window = UIApplication.SharedApplication.KeyWindow;
                    var vc = window.RootViewController;
                    while (vc.PresentedViewController != null)
                    {
                        vc = vc.PresentedViewController;
                    }
                    _adInterstitial.Present(vc);
                }
            };
            _adInterstitial.LoadRequest(request);

        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}