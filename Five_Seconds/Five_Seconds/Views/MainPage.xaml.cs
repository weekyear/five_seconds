using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Five_Seconds.Models;
using Xam.Plugin.SimpleAppIntro;

namespace Five_Seconds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [AdMaiora.RealXaml.Client.MainPage]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            AdMaiora.RealXaml.Client.AppManager.Init(this);

            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Main, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Main:
                        MenuPages.Add(id, new NavigationPage(new MissionsPage()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                    case (int)MenuItemType.AppIntro:
                        OpenAppIntro();
                        IsPresented = false;
                        return;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }

        //public void ToggleScreenLock()
        //{
        //    if (!ScreenLock.IsActive)
        //        ScreenLock.RequestActive();
        //    else
        //        ScreenLock.RequestRelease();
        //}

        public void OpenAppIntro()
        {
            var welcomePage = new SimpleAppIntro(new List<object>()
            {
                new Slide(new SlideConfig("5초의 법칙의 세계에 오신걸 환영합니다~", "저희 어플 사용하는 법을 간단히 알려드릴게요~", "ic_hospitality.png",
                "#f8bbd0", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("우선 알람이 있어야죠?", "메인 화면 우측 상단의 +버튼을 클릭하시고 알람을 설정해주세요~", "ic_add_alarm.png",
                "#d1c4e9", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("알람이 울렸어요!", "알람이 울리면 버튼을 누르고 해야 될 것을 말해주세요!", "ic_voice_recognition.png",
                "#c5cae9", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("힝..폰이 못 알아들어요ㅠ^ㅠ", "텍스트창에 해야 될 것을 적고 버튼을 눌러주세요", "ic_complete.png",
                "#bbdefb", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("5초를 세줄게요!!", "5초 카운트가 끝나기 전에 지체말고 바로 시작해요!", "ic_treadmill.png",
                "#b3e5fc", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
                new Slide(new SlideConfig("더 알고 싶은데..", "어플 안에서 5초의 법칙을 더 자세히 알아보세요~", "ic_search_phone.png",
                "#b2ebf2", "#FFFFFF", "#FFFFFF", FontAttributes.Bold, FontAttributes.Italic, 24, 16)),
            });

            welcomePage.DoneText = "시작";
            welcomePage.SkipText = "건너뛰기";
            welcomePage.NextText = "다음";
            welcomePage.BarColor = "#607D8B";
            welcomePage.SkipButtonBackgroundColor = "#00000000";
            welcomePage.DoneButtonBackgroundColor = "#80deea";
            welcomePage.NextButtonBackgroundColor = "#00000000";

            Navigation.PushModalAsync(welcomePage);
        }
    }
}