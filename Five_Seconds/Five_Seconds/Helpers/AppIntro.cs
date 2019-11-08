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
                new Slide(new SlideConfig("5초의 알람을 사용해주셔서 감사합니다!!", "저희 어플 사용하는 법을 간단히 알려드릴게요~", "ic_hospitality.png",
                "#f8bbd0", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("우선 알람이 있어야죠?", "메인 화면 우측 상단의 +버튼을 클릭하시고 알람을 추가해주세요~", "ic_add_alarm.png",
                "#d1c4e9", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("알람이 울렸어요!", "알람이 울리면 녹음 버튼을 누르고 '알람 이름'을 휴대폰에게 말해주세요!", "ic_voice_recognition.png",
                "#c5cae9", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("폰이 못 알아들어요ㅠ^ㅠ", "텍스트창에 알람 이름을 적고 시작 버튼을 눌러주세요", "ic_complete.png",
                "#bbdefb", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("5초를 세줄게요!!", "5초 카운트가 끝나기 전에 지체말고 바로 시작합시다!", "ic_treadmill.png",
                "#b3e5fc", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("더 알고 싶은데..", "어플 내에서 5초의 법칙을 더 자세히 알아보실 수 있습니다~", "ic_search_phone.png",
                "#b2ebf2", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
            })
            {
                Vibrate = false,
                DoneText = "시작",
                SkipText = "건너뛰기",
                NextText = "다음",
                BarColor = "#607D8B",
                SkipButtonBackgroundColor = "#00000000",
                DoneButtonBackgroundColor = "#80deea",
                NextButtonBackgroundColor = "#00000000"
            };

            return welcomePage;
        }
    }
}
