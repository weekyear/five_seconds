using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Five_Seconds.Services;
using Five_Seconds.Views;
using Five_Seconds.Repository;
using Five_Seconds.Models;
using Xam.Plugin.SimpleAppIntro;
using System.Collections.Generic;
using Xamarin.Essentials;
using SQLite;
using System.ComponentModel;

namespace Five_Seconds
{
    public partial class App : Application
    {
        public static ItemDatabaseGeneric ItemDatabase { get; } = new ItemDatabaseGeneric();
        private bool isFirst = Preferences.Get("isFirst", true);

        public App()
        {
            AdMaiora.RealXaml.Client.AppManager.Init(this);

            DependencyService.Register<INavigation>();
            DependencyService.Register<IMissionsRepository>();
            DependencyService.Register<IMissionService>();
            DependencyService.Register<IMessageBoxService>();
            DependencyService.Register<ISpeechToText>();

            InitializeComponent();

            MainPage = new MainPage();

            if (isFirst)
            {
                OpenAppIntro();
            }

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

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

            MainPage.Navigation.PushModalAsync(welcomePage);

            Preferences.Set("isFirst", false);
        }

        private static IMissionsRepository missionsRepo;
        public static IMissionsRepository MissionsRepo
        {
            get
            {
                if (missionsRepo == null)
                {
                    missionsRepo = new MissionsRepository();
                }
                return missionsRepo;
            }
        }

        private static IMissionService service;
        public static IMissionService Service
        {
            get
            {
                if (service == null)
                {
                    service = new MissionService(App.MissionsRepo);
                }
                return service;
            }
        }

        private static IAlarmToneRepository alarmToneRepo;
        public static IAlarmToneRepository AlarmToneRepo
        {
            get
            {
                if (alarmToneRepo == null)
                {
                    alarmToneRepo = new AlarmToneRepository();
                }
                return alarmToneRepo;
            }
        }
    }
}
