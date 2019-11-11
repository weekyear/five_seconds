using Five_Seconds.Resources;
using Five_Seconds.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xam.Plugin.SimpleAppIntro;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Five_Seconds.Helpers
{
    public class AppIntro
    {
        public static SimpleAppIntro CreateAppIntro()
        {
            var welcomePage = new SimpleAppIntro(new List<object>()
            {
                new Slide(new SlideConfig(AppResources.Brief_01, AppResources.Brief_Detail_01, "ic_hospitality.png",
                "#f8bbd0", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig(AppResources.Brief_02, AppResources.Brief_Detail_02, "ic_add_alarm.png",
                "#d1c4e9", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig(AppResources.Brief_03, AppResources.Brief_Detail_03, "ic_voice_recognition.png",
                "#c5cae9", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig(AppResources.Brief_04, AppResources.Brief_Detail_04, "ic_complete.png",
                "#bbdefb", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig(AppResources.Brief_05, AppResources.Brief_Detail_05, "ic_treadmill.png",
                "#b3e5fc", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig(AppResources.Brief_06, AppResources.Brief_Detail_06, "ic_search_phone.png",
                "#b2ebf2", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
            })
            {
                Vibrate = false,
                DoneText = AppResources.Done,
                SkipText = AppResources.Skip,
                NextText = AppResources.Next,
                BarColor = "#607D8B",
                SkipButtonBackgroundColor = "#00000000",
                DoneButtonBackgroundColor = "#80deea",
                NextButtonBackgroundColor = "#00000000"
            };

            return welcomePage;
        }
    }
}
