using System;
using Xamarin.Forms;
using Five_Seconds.Services;
using Five_Seconds.Views;
using Five_Seconds.Repository;
using Five_Seconds.Models;
using System.Collections.Generic;
using Xamarin.Essentials;
using Five_Seconds.Helpers;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Application = Xamarin.Forms.Application;
using NavigationPage = Xamarin.Forms.NavigationPage;
using System.Globalization;

namespace Five_Seconds
{
    public partial class App : Application
    {
        public static ItemDatabaseGeneric ItemDatabase { get; } = new ItemDatabaseGeneric(DependencyService.Get<IDatabase>().DBConnect());
        private readonly bool isNotFirst = Preferences.Get(nameof(isNotFirst), false);
        private readonly int MaxAlarmId = Preferences.Get(nameof(MaxAlarmId), 3);

        public App()
        {
            //SetCulture("en-US");

            AdMaiora.RealXaml.Client.AppManager.Init(this);

            DependencyService.Register<INavigation>();
            DependencyService.Register<IAlarmsRepository>();
            DependencyService.Register<IAlarmService>();
            DependencyService.Register<IMessageBoxService>();
            DependencyService.Register<ISpeechToText>();

            InitializeComponent();

            var navigationPage = new NavigationPage(new AlarmsPage());

            navigationPage.On<iOS>().SetPrefersLargeTitles(true);

            MainPage = navigationPage;

            if (!isNotFirst)
            {
                var everdayBool = new bool[] { true, true, true, true, true, true, true };

                var ListInitAlarm = new List<Alarm>()
                {
                    new Alarm() { Name = "일어나서 이불개자", Days = new DaysOfWeek(everdayBool), Time = new TimeSpan(7, 0, 0), IsActive = false },
                    new Alarm() { Name = "아침 운동 좋아", Days = new DaysOfWeek(everdayBool), Time = new TimeSpan(7, 30, 0), IsActive = false },
                    new Alarm() { Name = "일단 침대에 눕자", Days = new DaysOfWeek(everdayBool), Time = new TimeSpan(23, 30, 0), IsActive = false }
                };

                foreach (var alarm in ListInitAlarm)
                {
                    service.SaveAlarmAtLocal(alarm);
                }

                var welcomePage = AppIntro.CreateAppIntro();
                welcomePage.Vibrate = false;
                MainPage.Navigation.PushModalAsync(welcomePage);

                Preferences.Set(nameof(isNotFirst), true);
            }

            Alarm.IsInitFinished = true;
        }

        private void SetCulture(string culture)
        {
            CultureInfo myCulture = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = myCulture;
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
        

        private static IAlarmsRepository alarmsRepo;
        public static IAlarmsRepository AlarmsRepo
        {
            get
            {
                if (alarmsRepo == null)
                {
                    alarmsRepo = new AlarmsRepository(ItemDatabase);
                }
                return alarmsRepo;
            }
        }

        private static IAlarmService service;
        public static IAlarmService Service
        {
            get
            {
                if (service == null)
                {
                    service = new AlarmService(AlarmsRepo);
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
                    alarmToneRepo = new AlarmToneRepository(ItemDatabase);
                }
                return alarmToneRepo;
            }
        }
    }
}
